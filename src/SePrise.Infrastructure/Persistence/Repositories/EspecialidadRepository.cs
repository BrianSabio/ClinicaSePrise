using Microsoft.EntityFrameworkCore;
using SePrise.Domain.Entities;
using SePrise.Domain.Repositories;

namespace SePrise.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementación concreta del repositorio de <see cref="Especialidad"/> usando Entity Framework Core.
/// Encapsula todas las operaciones de acceso a datos para el catálogo de especialidades.
/// </summary>
public class EspecialidadRepository : IEspecialidadRepository
{
    private readonly SePriseDbContext _context;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="EspecialidadRepository"/>.
    /// </summary>
    /// <param name="context">Contexto de base de datos de EF Core.</param>
    /// <exception cref="ArgumentNullException">Si <paramref name="context"/> es nulo.</exception>
    public EspecialidadRepository(SePriseDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc/>
    public async Task<Especialidad?> GetByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("El ID de la especialidad debe ser mayor a 0.", nameof(id));

        return await _context.Especialidades
            .FirstOrDefaultAsync(e => e.IdEspecialidad == id);
    }

    /// <inheritdoc/>
    public async Task<Especialidad?> GetByNombreAsync(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre no puede ser nulo ni vacío.", nameof(nombre));

        // EF Core traduce .ToLower() a LOWER() en SQL → comparación case-insensitive
        string nombreNormalizado = nombre.Trim().ToLower();
        return await _context.Especialidades
            .FirstOrDefaultAsync(e => e.Nombre.ToLower() == nombreNormalizado);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Especialidad>> GetAllActivasAsync()
    {
        // Solo especialidades activas, ordenadas alfabéticamente
        return await _context.Especialidades
            .Where(e => e.Activo)
            .OrderBy(e => e.Nombre)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<int> AddAsync(Especialidad especialidad)
    {
        if (especialidad is null)
            throw new ArgumentNullException(nameof(especialidad));

        await _context.Especialidades.AddAsync(especialidad);
        return especialidad.IdEspecialidad;
    }

    /// <inheritdoc/>
    public Task UpdateAsync(Especialidad especialidad)
    {
        if (especialidad is null)
            throw new ArgumentNullException(nameof(especialidad));

        _context.Especialidades.Update(especialidad);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
