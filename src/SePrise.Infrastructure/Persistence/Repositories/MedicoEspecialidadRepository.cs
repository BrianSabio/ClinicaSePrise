using Microsoft.EntityFrameworkCore;
using SePrise.Domain.Entities;
using SePrise.Domain.Repositories;

namespace SePrise.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementación concreta del repositorio de <see cref="MedicoEspecialidad"/> usando Entity Framework Core.
/// Gestiona la relación N:N entre médicos y especialidades.
/// </summary>
public class MedicoEspecialidadRepository : IMedicoEspecialidadRepository
{
    private readonly SePriseDbContext _context;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="MedicoEspecialidadRepository"/>.
    /// </summary>
    /// <param name="context">Contexto de base de datos de EF Core.</param>
    /// <exception cref="ArgumentNullException">Si <paramref name="context"/> es nulo.</exception>
    public MedicoEspecialidadRepository(SePriseDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc/>
    public async Task<MedicoEspecialidad?> GetByIdsAsync(int idMedico, int idEspecialidad)
    {
        if (idMedico <= 0)
            throw new ArgumentException("El ID del médico debe ser mayor a 0.", nameof(idMedico));

        if (idEspecialidad <= 0)
            throw new ArgumentException("El ID de la especialidad debe ser mayor a 0.", nameof(idEspecialidad));

        // La PK compuesta permite buscar directamente por ambas columnas
        return await _context.MedicoEspecialidades
            .Include(me => me.Medico)
            .Include(me => me.Especialidad)
            .FirstOrDefaultAsync(me => me.IdMedico == idMedico && me.IdEspecialidad == idEspecialidad);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<MedicoEspecialidad>> GetByMedicoAsync(int idMedico)
    {
        if (idMedico <= 0)
            throw new ArgumentException("El ID del médico debe ser mayor a 0.", nameof(idMedico));

        // Include Especialidad para cargar sus datos en una sola query (evita N+1)
        return await _context.MedicoEspecialidades
            .Where(me => me.IdMedico == idMedico)
            .Include(me => me.Especialidad)
            .OrderBy(me => me.Especialidad.Nombre)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<MedicoEspecialidad>> GetByEspecialidadAsync(int idEspecialidad)
    {
        if (idEspecialidad <= 0)
            throw new ArgumentException("El ID de la especialidad debe ser mayor a 0.", nameof(idEspecialidad));

        // Include Medico para cargar sus datos en una sola query (evita N+1)
        return await _context.MedicoEspecialidades
            .Where(me => me.IdEspecialidad == idEspecialidad)
            .Include(me => me.Medico)
            .OrderBy(me => me.Medico.Apellido)
            .ThenBy(me => me.Medico.Nombre)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task AddAsync(MedicoEspecialidad medicoEspecialidad)
    {
        if (medicoEspecialidad is null)
            throw new ArgumentNullException(nameof(medicoEspecialidad));

        await _context.MedicoEspecialidades.AddAsync(medicoEspecialidad);
    }

    /// <inheritdoc/>
    public async Task RemoveAsync(int idMedico, int idEspecialidad)
    {
        if (idMedico <= 0)
            throw new ArgumentException("El ID del médico debe ser mayor a 0.", nameof(idMedico));

        if (idEspecialidad <= 0)
            throw new ArgumentException("El ID de la especialidad debe ser mayor a 0.", nameof(idEspecialidad));

        // Buscar la asociación para eliminarla
        var asociacion = await _context.MedicoEspecialidades
            .FirstOrDefaultAsync(me => me.IdMedico == idMedico && me.IdEspecialidad == idEspecialidad);

        // Si no existe, ignoramos silenciosamente (idempotente)
        if (asociacion is not null)
            _context.MedicoEspecialidades.Remove(asociacion);
    }

    /// <inheritdoc/>
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
