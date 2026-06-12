namespace SePrise.Application.Services.Implementations;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using SePrise.Domain.Repositories;
using SePrise.Domain.Exceptions;
using SePrise.Application.DTOs.Atencion;
using SePrise.Application.Services.Interfaces;
using SePrise.Domain.ValueObjects;
public class AtencionService : IAtencionService
{
    private readonly IAtencionRepository _atencionRepository;
    private readonly ITurnoRepository _turnoRepository;
    private readonly IMapper _mapper;
public AtencionService(
        IAtencionRepository atencionRepository,
        ITurnoRepository turnoRepository,
        IMapper mapper)
    {
        _atencionRepository = atencionRepository ?? throw new ArgumentNullException(nameof(atencionRepository));
        _turnoRepository = turnoRepository ?? throw new ArgumentNullException(nameof(turnoRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    public async Task<AtencionDTO> IniciarAtencionAsync(int idAtencion)
    {
        if (idAtencion <= 0)
            throw new ArgumentException("ID de atención debe ser > 0", nameof(idAtencion));

        var atencion = await _atencionRepository.GetByIdAsync(idAtencion);
        if (atencion == null)
            throw new AtencionException($"Atención con ID {idAtencion} no encontrada");
        atencion.ProgresarAEnProgreso(DateTime.UtcNow); // Lanza AtencionException si no puede
        await _atencionRepository.UpdateAsync(atencion);
        await _atencionRepository.SaveChangesAsync();

        return _mapper.Map<AtencionDTO>(atencion);
    }
    public async Task<AtencionDTO> FinalizarAtencionAsync(int idAtencion, string? notas = null)
    {
        if (idAtencion <= 0)
            throw new ArgumentException("ID de atención debe ser > 0", nameof(idAtencion));

        var atencion = await _atencionRepository.GetByIdAsync(idAtencion);
        if (atencion == null)
            throw new AtencionException($"Atención con ID {idAtencion} no encontrada");

        // Finalizar atención
        atencion.Finalizar(DateTime.UtcNow, notas); // Lanza AtencionException si no puede

        // Cascada: marcar turno como atendido si existe
        if (atencion.IdTurno.HasValue)
        {
            var turno = await _turnoRepository.GetByIdAsync(atencion.IdTurno.Value);
            if (turno != null)
            {
                turno.MarcarComoAtendido();
                await _turnoRepository.UpdateAsync(turno);
            }
        }
        await _atencionRepository.UpdateAsync(atencion);
        await _atencionRepository.SaveChangesAsync();

        return _mapper.Map<AtencionDTO>(atencion);
    }
    public async Task<AtencionDTO> CancelarAtencionAsync(int idAtencion)
    {
        if (idAtencion <= 0)
            throw new ArgumentException("ID de atención debe ser > 0", nameof(idAtencion));

        var atencion = await _atencionRepository.GetByIdAsync(idAtencion);
        if (atencion == null)
            throw new AtencionException($"Atención con ID {idAtencion} no encontrada");

        // Cancelar
        atencion.Cancelar(); // Lanza AtencionException si no puede

        // Cascada: cancelar turno si existe
        if (atencion.IdTurno.HasValue)
        {
            var turno = await _turnoRepository.GetByIdAsync(atencion.IdTurno.Value);
            if (turno != null && turno.Estado.PuedeCancelarse())
            {
                turno.CancelarTurno();
                await _turnoRepository.UpdateAsync(turno);
            }
        }
        await _atencionRepository.UpdateAsync(atencion);
        await _atencionRepository.SaveChangesAsync();

        return _mapper.Map<AtencionDTO>(atencion);
    }
    public async Task<AtencionDTO> ActualizarNotasAsync(int idAtencion, string notas)
    {
        if (idAtencion <= 0)
            throw new ArgumentException("ID de atención debe ser > 0", nameof(idAtencion));

        if (string.IsNullOrWhiteSpace(notas))
            throw new ArgumentException("Notas no pueden estar vacías", nameof(notas));

        var atencion = await _atencionRepository.GetByIdAsync(idAtencion);
        if (atencion == null)
            throw new AtencionException($"Atención con ID {idAtencion} no encontrada");

        // Actualizar notas
        atencion.ActualizarNotas(notas); // Lanza AtencionException si está terminal
        await _atencionRepository.UpdateAsync(atencion);
        await _atencionRepository.SaveChangesAsync();

        return _mapper.Map<AtencionDTO>(atencion);
    }
    public async Task<IEnumerable<AtencionDTO>> ObtenerPacientesEsperandoAsync(int idMedico)
    {
        if (idMedico <= 0)
            throw new ArgumentException("ID de médico debe ser > 0", nameof(idMedico));

        var atenciones = await _atencionRepository.GetAtendiendoAsync(idMedico);
        return _mapper.Map<IEnumerable<AtencionDTO>>(atenciones);
    }
public async Task<AtencionDTO> ObtenerAtencionAsync(int idAtencion)
    {
        if (idAtencion <= 0) throw new ArgumentException("ID inválido");
        var atencion = await _atencionRepository.GetByIdAsync(idAtencion);
        if (atencion == null) throw new AtencionException($"Atención {idAtencion} no encontrada");
        return _mapper.Map<AtencionDTO>(atencion);
    }
    public async Task<IEnumerable<AtencionDTO>> ListarAtencionesAsync(string? estado, int? idPaciente, int? idMedico)
    {
        var atenciones = await _atencionRepository.GetAllAsync();
        
        if (!string.IsNullOrEmpty(estado) && Enum.TryParse<SePrise.Domain.ValueObjects.EstadoAtencion>(estado, out var enumEstado))
        {
            atenciones = atenciones.Where(a => a.Estado == enumEstado);
        }
        if (idPaciente.HasValue)
        {
            atenciones = atenciones.Where(a => a.IdPaciente == idPaciente.Value);
        }
        if (idMedico.HasValue)
        {
            atenciones = atenciones.Where(a => a.IdMedico == idMedico.Value);
        }

        return _mapper.Map<IEnumerable<AtencionDTO>>(atenciones);
    }
}


