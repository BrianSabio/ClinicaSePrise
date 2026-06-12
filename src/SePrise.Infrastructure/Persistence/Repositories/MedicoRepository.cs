using Microsoft.EntityFrameworkCore;
using SePrise.Domain.Entities;
using SePrise.Domain.Repositories;

namespace SePrise.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementación concreta del repositorio de <see cref="Medico"/> usando Entity Framework Core.
/// Encapsula todas las operaciones de acceso a datos para médicos y sus especialidades.
/// </summary>
public class MedicoRepository : IMedicoRepository
{
    private readonly SePriseDbContext _context;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="MedicoRepository"/>.
    /// </summary>
    /// <param name="context">Contexto de base de datos de EF Core.</param>
    /// <exception cref="ArgumentNullException">Si <paramref name="context"/> es nulo.</exception>
    public MedicoRepository(SePriseDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc/>
    public async Task<Medico?> GetByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("El ID del médico debe ser mayor a 0.", nameof(id));

        return await _context.Medicos
            .FirstOrDefaultAsync(m => m.IdMedico == id);
    }

    /// <inheritdoc/>
    public async Task<Medico?> GetByNumeroMatriculaAsync(string numeroMatricula)
    {
        if (string.IsNullOrWhiteSpace(numeroMatricula))
            throw new ArgumentException("El número de matrícula no puede ser nulo ni vacío.", nameof(numeroMatricula));

        // La matrícula es única y no requiere normalización de mayúsculas
        return await _context.Medicos
            .FirstOrDefaultAsync(m => m.NumeroMatricula == numeroMatricula.Trim());
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Medico>> GetAllActivosAsync()
    {
        // Solo médicos activos, ordenados apellido → nombre
        return await _context.Medicos
            .Where(m => m.Activo)
            .OrderBy(m => m.Apellido)
            .ThenBy(m => m.Nombre)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Especialidad>> GetEspecialidadesByMedicoAsync(int idMedico)
    {
        if (idMedico <= 0)
            throw new ArgumentException("El ID del médico debe ser mayor a 0.", nameof(idMedico));

        // Join a través de la tabla asociativa MedicoEspecialidades
        // Include Especialidad para cargar sus datos en una sola query (evita N+1)
        return await _context.MedicoEspecialidades
            .Where(me => me.IdMedico == idMedico)
            .Include(me => me.Especialidad)
            .Select(me => me.Especialidad)
            .OrderBy(e => e.Nombre)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<int> AddAsync(Medico medico)
    {
        if (medico is null)
            throw new ArgumentNullException(nameof(medico));

        await _context.Medicos.AddAsync(medico);
        return medico.IdMedico;
    }

    /// <inheritdoc/>
    public Task UpdateAsync(Medico medico)
    {
        if (medico is null)
            throw new ArgumentNullException(nameof(medico));

        _context.Medicos.Update(medico);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
