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

/// <summary>
/// Servicio de aplicación para gestión de atenciones médicas.
/// Maneja el flujo de pacientes en consultorio (inicio, finalización, cancelación).
/// </summary>
public class AtencionService : IAtencionService
{
    private readonly IAtencionRepository _atencionRepository;
    private readonly ITurnoRepository _turnoRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Inicializa una nueva instancia de AtencionService.
    /// </summary>
    public AtencionService(
        IAtencionRepository atencionRepository,
        ITurnoRepository turnoRepository,
        IMapper mapper)
    {
        _atencionRepository = atencionRepository ?? throw new ArgumentNullException(nameof(atencionRepository));
        _turnoRepository = turnoRepository ?? throw new ArgumentNullException(nameof(turnoRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// Inicia una atención (médico comienza a atender paciente).
    /// Cambia atención de Acreditada a EnProgreso.
    /// </summary>
    /// <param name="idAtencion">ID de la atención.</param>
    /// <returns>DTO de la atención iniciada.</returns>
    /// <exception cref="ArgumentException">Si idAtencion &lt;= 0.</exception>
    /// <exception cref="AtencionException">Si atención no existe o no está Acreditada.</exception>
    public async Task<AtencionDTO> IniciarAtencionAsync(int idAtencion)
    {
        if (idAtencion <= 0)
            throw new ArgumentException("ID de atención debe ser > 0", nameof(idAtencion));

        var atencion = await _atencionRepository.GetByIdAsync(idAtencion);
        if (atencion == null)
            throw new AtencionException($"Atención con ID {idAtencion} no encontrada");

        // Cambiar estado
        atencion.ProgresarAEnProgreso(DateTime.UtcNow); // Lanza AtencionException si no puede

        // Persistir
        await _atencionRepository.UpdateAsync(atencion);
        await _atencionRepository.SaveChangesAsync();

        return _mapper.Map<AtencionDTO>(atencion);
    }

    /// <summary>
    /// Finaliza una atención (médico completa la consulta).
    /// Cambia atención de EnProgreso a Finalizada.
    /// Si existe turno asociado, lo marca como Atendido.
    /// </summary>
    /// <param name="idAtencion">ID de la atención.</param>
    /// <param name="notas">Notas del médico (opcional).</param>
    /// <returns>DTO de la atención finalizada.</returns>
    /// <exception cref="ArgumentException">Si idAtencion &lt;= 0.</exception>
    /// <exception cref="AtencionException">Si atención no existe o no está EnProgreso.</exception>
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

        // Persistir
        await _atencionRepository.UpdateAsync(atencion);
        await _atencionRepository.SaveChangesAsync();

        return _mapper.Map<AtencionDTO>(atencion);
    }

    /// <summary>
    /// Cancela una atención en progreso o acreditada.
    /// </summary>
    /// <param name="idAtencion">ID de la atención.</param>
    /// <returns>DTO de la atención cancelada.</returns>
    /// <exception cref="ArgumentException">Si idAtencion &lt;= 0.</exception>
    /// <exception cref="AtencionException">Si atención no existe o ya está terminal.</exception>
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

        // Persistir
        await _atencionRepository.UpdateAsync(atencion);
        await _atencionRepository.SaveChangesAsync();

        return _mapper.Map<AtencionDTO>(atencion);
    }

    /// <summary>
    /// Actualiza las notas de una atención en progreso.
    /// Útil para que el médico agregue información mientras atiende.
    /// </summary>
    /// <param name="idAtencion">ID de la atención.</param>
    /// <param name="notas">Nuevas notas a agregar.</param>
    /// <returns>DTO de la atención actualizado.</returns>
    /// <exception cref="ArgumentException">Si parámetros son inválidos.</exception>
    /// <exception cref="AtencionException">Si atención no existe o está terminal.</exception>
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

        // Persistir
        await _atencionRepository.UpdateAsync(atencion);
        await _atencionRepository.SaveChangesAsync();

        return _mapper.Map<AtencionDTO>(atencion);
    }

    /// <summary>
    /// Obtiene la lista de pacientes esperando para un médico específico.
    /// Retorna atenciones en estado Acreditada o EnProgreso.
    /// </summary>
    /// <param name="idMedico">ID del médico.</param>
    /// <returns>Colección de DTOs de atenciones esperando.</returns>
    /// <exception cref="ArgumentException">Si idMedico &lt;= 0.</exception>
    public async Task<IEnumerable<AtencionDTO>> ObtenerPacientesEsperandoAsync(int idMedico)
    {
        if (idMedico <= 0)
            throw new ArgumentException("ID de médico debe ser > 0", nameof(idMedico));

        var atenciones = await _atencionRepository.GetAtendiendoAsync(idMedico);
        return _mapper.Map<IEnumerable<AtencionDTO>>(atenciones);
    }

    /// <summary>
    /// Obtiene una atención por ID.
    /// </summary>
    public async Task<AtencionDTO> ObtenerAtencionAsync(int idAtencion)
    {
        if (idAtencion <= 0) throw new ArgumentException("ID inválido");
        var atencion = await _atencionRepository.GetByIdAsync(idAtencion);
        if (atencion == null) throw new AtencionException($"Atención {idAtencion} no encontrada");
        return _mapper.Map<AtencionDTO>(atencion);
    }

    /// <summary>
    /// Lista atenciones filtrando.
    /// </summary>
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
