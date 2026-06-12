using System;
using System.Threading.Tasks;
using AutoMapper;
using SePrise.Domain.Aggregates;
using SePrise.Domain.Exceptions;
using SePrise.Domain.Repositories;
using SePrise.Domain.ValueObjects;
using SePrise.Application.DTOs.Atencion;
using SePrise.Application.Services.Interfaces;

namespace SePrise.Application.Services.Implementations;
public class AcreditacionService : IAcreditacionService
{
    private readonly ITurnoRepository _turnoRepository;
    private readonly IAtencionRepository _atencionRepository;
    private readonly IPacienteRepository _pacienteRepository;
    private readonly IMedicoRepository _medicoRepository;
    private readonly IMapper _mapper;

    public AcreditacionService(
        ITurnoRepository turnoRepository,
        IAtencionRepository atencionRepository,
        IPacienteRepository pacienteRepository,
        IMedicoRepository medicoRepository,
        IMapper mapper)
    {
        _turnoRepository = turnoRepository ?? throw new ArgumentNullException(nameof(turnoRepository));
        _atencionRepository = atencionRepository ?? throw new ArgumentNullException(nameof(atencionRepository));
        _pacienteRepository = pacienteRepository ?? throw new ArgumentNullException(nameof(pacienteRepository));
        _medicoRepository = medicoRepository ?? throw new ArgumentNullException(nameof(medicoRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<AtencionDTO> AcreditarPacienteDesdeReservaAsync(int idTurno, ModalidadPago modalidadPago)
    {
        if (idTurno <= 0) throw new ArgumentException("ID de turno debe ser > 0", nameof(idTurno));

        var turno = await _turnoRepository.GetByIdAsync(idTurno);
        if (turno == null) throw new TurnoException($"Turno con ID {idTurno} no encontrado");
        turno.ConfirmarTurno(); // lanza excepción si el estado no es Reservado

        // Crear la atención
        var atencion = AtencionAggregate.CrearDesdeConfirmacion(idTurno, turno.IdPaciente, turno.IdMedico, modalidadPago);

        await _turnoRepository.UpdateAsync(turno);
        await _atencionRepository.AddAsync(atencion);
        // en una única transacción atómica porque ambos repositorios comparten
        // la misma instancia de SePriseDbContext (Scoped).
        await _turnoRepository.SaveChangesAsync();

        var idAtencion = atencion.IdAtencion;
        var atencionCreada = await _atencionRepository.GetByIdAsync(idAtencion);
        return _mapper.Map<AtencionDTO>(atencionCreada);
    }

    public async Task<AtencionDTO> RegistrarDemandaEspontaneaAsync(AtencionCrearEspontaneaDTO dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));
        
        var paciente = await _pacienteRepository.GetByIdAsync(dto.IdPaciente);
        if (paciente == null) throw new PacienteException($"Paciente con ID {dto.IdPaciente} no encontrado");

        var medico = await _medicoRepository.GetByIdAsync(dto.IdMedico);
        if (medico == null) throw new MedicoException($"Médico con ID {dto.IdMedico} no encontrado");

        if (!Enum.TryParse<ModalidadPago>(dto.ModalidadPago, out var modalidadPago))
        {
            throw new ArgumentException("Modalidad de pago inválida");
        }

        var atencion = AtencionAggregate.CrearDemandaEspontanea(dto.IdPaciente, dto.IdMedico, modalidadPago);

        await _atencionRepository.AddAsync(atencion);
        await _atencionRepository.SaveChangesAsync();

        var idAtencion = atencion.IdAtencion;
        var atencionCreada = await _atencionRepository.GetByIdAsync(idAtencion);
        return _mapper.Map<AtencionDTO>(atencionCreada);
    }

    public async Task RegistrarNoAsistioAsync(int idTurno)
    {
        if (idTurno <= 0) throw new ArgumentException("ID de turno debe ser > 0", nameof(idTurno));

        var turno = await _turnoRepository.GetByIdAsync(idTurno);
        if (turno == null) throw new TurnoException($"Turno con ID {idTurno} no encontrado");

        var atencion = await _atencionRepository.GetByTurnoAsync(idTurno);
        if (atencion != null) throw new TurnoException("No se puede registrar inasistencia porque ya existe una atención asociada");

        turno.MarcarNoAsistio();

        await _turnoRepository.UpdateAsync(turno);
        await _turnoRepository.SaveChangesAsync();
    }
}


