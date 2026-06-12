using Microsoft.EntityFrameworkCore;
using SePrise.Domain.Aggregates;
using SePrise.Domain.Repositories;
using SePrise.Domain.ValueObjects;

namespace SePrise.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementación concreta del repositorio de <see cref="TurnoAggregate"/> usando Entity Framework Core.
/// Encapsula todas las operaciones de acceso a datos para el ciclo de vida de los turnos.
/// Incluye queries complejas para agendamiento y agenda diaria.
/// </summary>
public class TurnoRepository : ITurnoRepository
{
    private readonly SePriseDbContext _context;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="TurnoRepository"/>.
    /// </summary>
    /// <param name="context">Contexto de base de datos de EF Core.</param>
    /// <exception cref="ArgumentNullException">Si <paramref name="context"/> es nulo.</exception>
    public TurnoRepository(SePriseDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc/>
    public async Task<TurnoAggregate?> GetByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("El ID del turno debe ser mayor a 0.", nameof(id));

        // Carga todas las relaciones necesarias para la lógica de negocio en una sola query
        return await _context.Turnos
            .Include(t => t.Paciente)
            .Include(t => t.Medico)
            .Include(t => t.Especialidad)
            .Include(t => t.Sala)
            .Include(t => t.Atencion)
            .FirstOrDefaultAsync(t => t.IdTurno == id);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TurnoAggregate>> GetByPacienteAsync(int idPaciente)
    {
        if (idPaciente <= 0)
            throw new ArgumentException("El ID del paciente debe ser mayor a 0.", nameof(idPaciente));

        // Historial completo del paciente, del más reciente al más antiguo
        return await _context.Turnos
            .Where(t => t.IdPaciente == idPaciente)
            .Include(t => t.Medico)
            .Include(t => t.Especialidad)
            .OrderByDescending(t => t.FechaHoraInicio)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TurnoAggregate>> GetByMedicoYFechaAsync(int idMedico, DateTime fecha)
    {
        if (idMedico <= 0)
            throw new ArgumentException("El ID del médico debe ser mayor a 0.", nameof(idMedico));

        // Calcular el rango del día completo (00:00:00 → 23:59:59) de la fecha solicitada
        DateTime inicioDia = fecha.Date;
        DateTime finDia = inicioDia.AddDays(1).AddTicks(-1);

        // Agenda diaria del médico: todos los turnos en ese rango horario, ordenados por hora
        return await _context.Turnos
            .Where(t => t.IdMedico == idMedico
                     && t.FechaHoraInicio >= inicioDia
                     && t.FechaHoraInicio <= finDia)
            .Include(t => t.Paciente)
            .Include(t => t.Especialidad)
            .Include(t => t.Sala)
            .OrderBy(t => t.FechaHoraInicio)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TurnoAggregate>> GetDisponiblesAsync(
        int idEspecialidad,
        DateTime fechaDesde,
        DateTime fechaHasta)
    {
        if (idEspecialidad <= 0)
            throw new ArgumentException("El ID de la especialidad debe ser mayor a 0.", nameof(idEspecialidad));

        if (fechaDesde > fechaHasta)
            throw new ArgumentException("FechaDesde no puede ser posterior a FechaHasta.");

        // Turnos en estado Reservado (aún no confirmados ni cancelados)
        // para la especialidad solicitada dentro del rango de fechas
        return await _context.Turnos
            .Where(t => t.IdEspecialidad == idEspecialidad
                     && t.Estado == EstadoTurno.Reservado
                     && t.FechaHoraInicio >= fechaDesde
                     && t.FechaHoraInicio <= fechaHasta)
            .Include(t => t.Medico)
            .Include(t => t.Sala)
            .OrderBy(t => t.FechaHoraInicio)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TurnoAggregate>> GetByEstadoAsync(EstadoTurno estado)
    {
        return await _context.Turnos
            .Where(t => t.Estado == estado)
            .Include(t => t.Paciente)
            .Include(t => t.Medico)
            .OrderByDescending(t => t.FechaHoraInicio)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TurnoAggregate>> GetAllAsync()
    {
        // Devuelve todos los turnos con las relaciones necesarias para mostrar en la UI
        return await _context.Turnos
            .Include(t => t.Paciente)
            .Include(t => t.Medico)
            .Include(t => t.Especialidad)
            .Include(t => t.Sala)
            .OrderByDescending(t => t.FechaHoraInicio)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<int> AddAsync(TurnoAggregate turno)
    {
        if (turno is null)
            throw new ArgumentNullException(nameof(turno));

        await _context.Turnos.AddAsync(turno);
        return turno.IdTurno;
    }

    /// <inheritdoc/>
    public Task UpdateAsync(TurnoAggregate turno)
    {
        if (turno is null)
            throw new ArgumentNullException(nameof(turno));

        // EF Core trackea los cambios realizados en la entidad durante el ciclo de vida del contexto
        _context.Turnos.Update(turno);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
