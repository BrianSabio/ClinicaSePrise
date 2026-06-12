namespace SePrise.Application.Services.Implementations;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using SePrise.Domain.Repositories;
using SePrise.Domain.Exceptions;
using SePrise.Domain.Aggregates;
using SePrise.Domain.ValueObjects;
using SePrise.Application.DTOs.Turno;
using SePrise.Application.Services.Interfaces;
public class AgendamientoService : IAgendamientoService
{
    private readonly ITurnoRepository _turnoRepository;
    private readonly IPacienteRepository _pacienteRepository;
    private readonly IMedicoRepository _medicoRepository;
    private readonly IEspecialidadRepository _especialidadRepository;
    private readonly ISalaRepository _salaRepository;
    private readonly IMedicoEspecialidadRepository _medicoEspecialidadRepository;
    private readonly IAtencionRepository _atencionRepository;
    private readonly IMapper _mapper;
public AgendamientoService(
        ITurnoRepository turnoRepository,
        IPacienteRepository pacienteRepository,
        IMedicoRepository medicoRepository,
        IEspecialidadRepository especialidadRepository,
        ISalaRepository salaRepository,
        IMedicoEspecialidadRepository medicoEspecialidadRepository,
        IAtencionRepository atencionRepository,
        IMapper mapper)
    {
        _turnoRepository = turnoRepository ?? throw new ArgumentNullException(nameof(turnoRepository));
        _pacienteRepository = pacienteRepository ?? throw new ArgumentNullException(nameof(pacienteRepository));
        _medicoRepository = medicoRepository ?? throw new ArgumentNullException(nameof(medicoRepository));
        _especialidadRepository = especialidadRepository ?? throw new ArgumentNullException(nameof(especialidadRepository));
        _salaRepository = salaRepository ?? throw new ArgumentNullException(nameof(salaRepository));
        _medicoEspecialidadRepository = medicoEspecialidadRepository ?? throw new ArgumentNullException(nameof(medicoEspecialidadRepository));
        _atencionRepository = atencionRepository ?? throw new ArgumentNullException(nameof(atencionRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
public async Task<TurnoDTO> ObtenerTurnoAsync(int idTurno)
    {
        if (idTurno <= 0) throw new ArgumentException("ID de turno inválido");
        var turno = await _turnoRepository.GetByIdAsync(idTurno);
        if (turno == null) throw new TurnoException($"Turno con ID {idTurno} no encontrado");
        return _mapper.Map<TurnoDTO>(turno);
    }
    public async Task<IEnumerable<TurnoDTO>> ListarTurnosPorPacienteAsync(int idPaciente)
    {
        if (idPaciente <= 0) throw new ArgumentException("ID de paciente inválido");
        var turnos = await _turnoRepository.GetByPacienteAsync(idPaciente);
        return _mapper.Map<IEnumerable<TurnoDTO>>(turnos);
    }
    public async Task<IEnumerable<TurnoDTO>> ListarTurnosDelDiaPorMedicoAsync(int idMedico, DateTime fecha)
    {
        if (idMedico <= 0) throw new ArgumentException("ID de médico inválido");
        var turnos = await _turnoRepository.GetByMedicoYFechaAsync(idMedico, fecha);
        return _mapper.Map<IEnumerable<TurnoDTO>>(turnos);
    }
    public async Task<IEnumerable<TurnoDTO>> ListarTodosTurnosAsync()
    {
        var turnos = await _turnoRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<TurnoDTO>>(turnos);
    }
    public async Task<TurnoDTO> CrearTurnoAsync(TurnoCrearDTO dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));
        var turno = _mapper.Map<TurnoAggregate>(dto);

        // Validaciones de coherencia
        var paciente = await _pacienteRepository.GetByIdAsync(turno.IdPaciente);
        if (paciente == null)
            throw new TurnoException($"Paciente con ID {turno.IdPaciente} no encontrado");

        var medico = await _medicoRepository.GetByIdAsync(turno.IdMedico);
        if (medico == null)
            throw new TurnoException($"Médico con ID {turno.IdMedico} no encontrado");

        var especialidad = await _especialidadRepository.GetByIdAsync(turno.IdEspecialidad);
        if (especialidad == null)
            throw new TurnoException($"Especialidad con ID {turno.IdEspecialidad} no encontrada");
        var medicoEspecs = await _medicoEspecialidadRepository.GetByMedicoAsync(turno.IdMedico);
        if (!medicoEspecs.Any(me => me.IdEspecialidad == turno.IdEspecialidad))
            throw new TurnoException($"Médico {medico.Nombre} no tiene asignada la especialidad {especialidad.Nombre}");

        var sala = await _salaRepository.GetByIdAsync(turno.IdSala);
        if (sala == null)
            throw new TurnoException($"Sala con ID {turno.IdSala} no encontrada");
        var turnosExistentes = await _turnoRepository.GetByMedicoYFechaAsync(turno.IdMedico, turno.FechaHoraInicio);
        var tieneOverlap = turnosExistentes.Any(t =>
            t.FechaHoraInicio <= turno.FechaHoraInicio &&
            t.FechaHoraInicio.AddMinutes(t.DuracionMinutos) > turno.FechaHoraInicio &&
            t.Estado != EstadoTurno.Cancelado &&
            t.Estado != EstadoTurno.NoAsistio &&
            t.Estado != EstadoTurno.Reprogramado
        );
        if (tieneOverlap)
            throw new TurnoException($"Médico {medico.Nombre} no está disponible en ese horario");
        await _turnoRepository.AddAsync(turno);
        await _turnoRepository.SaveChangesAsync();
        var idGenerado = turno.IdTurno;
        var turnoCreado = await _turnoRepository.GetByIdAsync(idGenerado);
        return _mapper.Map<TurnoDTO>(turnoCreado);
    }
    public async Task<TurnoDTO> ConfirmarTurnoAsync(int idTurno)
    {
        if (idTurno <= 0)
            throw new ArgumentException("ID de turno debe ser > 0", nameof(idTurno));

        var turno = await _turnoRepository.GetByIdAsync(idTurno);
        if (turno == null)
            throw new TurnoException($"Turno con ID {idTurno} no encontrado");
        turno.ConfirmarTurno(); // Lanza TurnoException si no puede
        await _turnoRepository.UpdateAsync(turno);
        await _turnoRepository.SaveChangesAsync();

        return _mapper.Map<TurnoDTO>(turno);
    }
    public async Task CancelarTurnoAsync(int idTurno)
    {
        if (idTurno <= 0)
            throw new ArgumentException("ID de turno debe ser > 0", nameof(idTurno));

        var turno = await _turnoRepository.GetByIdAsync(idTurno);
        if (turno == null)
            throw new TurnoException($"Turno con ID {idTurno} no encontrado");
        turno.CancelarTurno(); // Lanza TurnoException si no puede

        // Cascada: cancelar atencion si existe
        var atencion = await _atencionRepository.GetByTurnoAsync(idTurno);
        if (atencion != null && !atencion.Estado.EsTerminal())
        {
            atencion.Cancelar();
            await _atencionRepository.UpdateAsync(atencion);
        }
        await _turnoRepository.UpdateAsync(turno);
        await _turnoRepository.SaveChangesAsync();
    }
    public async Task<TurnoDTO> ReprogramarTurnoAsync(int idTurno, TurnoReprogramarDTO dto)
    {
        if (idTurno <= 0)
            throw new ArgumentException("ID de turno debe ser > 0", nameof(idTurno));

        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var turno = await _turnoRepository.GetByIdAsync(idTurno);
        if (turno == null)
            throw new TurnoException($"Turno con ID {idTurno} no encontrado");
        turno.ReprogramarTurno(); // Lanza TurnoException si no puede

        // Crear NUEVO turno con la nueva fecha/hora
        var nuevoTurno = TurnoAggregate.Crear(
            turno.IdPaciente,
            turno.IdMedico,
            turno.IdEspecialidad,
            turno.IdSala,
            dto.FechaHoraInicio,
            dto.DuracionMinutos
        );
        var turnosExistentes = await _turnoRepository.GetByMedicoYFechaAsync(nuevoTurno.IdMedico, nuevoTurno.FechaHoraInicio);
        var tieneOverlap = turnosExistentes.Any(t =>
            t.IdTurno != turno.IdTurno && // Excluir el turno original por las dudas, aunque está como Reprogramado
            t.FechaHoraInicio <= nuevoTurno.FechaHoraInicio &&
            t.FechaHoraInicio.AddMinutes(t.DuracionMinutos) > nuevoTurno.FechaHoraInicio &&
            t.Estado != EstadoTurno.Cancelado &&
            t.Estado != EstadoTurno.NoAsistio &&
            t.Estado != EstadoTurno.Reprogramado
        );

        if (tieneOverlap)
            throw new TurnoException($"Médico no está disponible en el nuevo horario propuesto");

        // Cascada: cancelar atencion si existe
        var atencion = await _atencionRepository.GetByTurnoAsync(idTurno);
        if (atencion != null && !atencion.Estado.EsTerminal())
        {
            atencion.Cancelar();
            await _atencionRepository.UpdateAsync(atencion);
        }
        await _turnoRepository.UpdateAsync(turno);
        await _turnoRepository.AddAsync(nuevoTurno);
        await _turnoRepository.SaveChangesAsync();

        var idNuevo = nuevoTurno.IdTurno;
        var turnoCreado = await _turnoRepository.GetByIdAsync(idNuevo);
        return _mapper.Map<TurnoDTO>(turnoCreado);
    }
}


