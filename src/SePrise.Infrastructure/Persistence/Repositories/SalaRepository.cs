using Microsoft.EntityFrameworkCore;
using SePrise.Domain.Entities;
using SePrise.Domain.Repositories;

namespace SePrise.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementación concreta del repositorio de <see cref="Sala"/> usando Entity Framework Core.
/// Encapsula todas las operaciones de acceso a datos para salas y consultorios de la clínica.
/// </summary>
public class SalaRepository : ISalaRepository
{
    private readonly SePriseDbContext _context;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="SalaRepository"/>.
    /// </summary>
    /// <param name="context">Contexto de base de datos de EF Core.</param>
    /// <exception cref="ArgumentNullException">Si <paramref name="context"/> es nulo.</exception>
    public SalaRepository(SePriseDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc/>
    public async Task<Sala?> GetByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("El ID de la sala debe ser mayor a 0.", nameof(id));

        return await _context.Salas
            .FirstOrDefaultAsync(s => s.IdSala == id);
    }

    /// <inheritdoc/>
    public async Task<Sala?> GetByNumeroAsync(string numero)
    {
        if (string.IsNullOrWhiteSpace(numero))
            throw new ArgumentException("El número de sala no puede ser nulo ni vacío.", nameof(numero));

        return await _context.Salas
            .FirstOrDefaultAsync(s => s.Numero == numero.Trim());
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Sala>> GetAllActivasByTipoAsync(TipoSala tipo)
    {
        // Filtrar por tipo de sala (enum almacenado como int en BD)
        return await _context.Salas
            .Where(s => s.Activo && s.TipoSala == tipo)
            .OrderBy(s => s.Numero)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Sala>> GetAllActivasAsync()
    {
        return await _context.Salas
            .Where(s => s.Activo)
            .OrderBy(s => s.Numero)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<int> AddAsync(Sala sala)
    {
        if (sala is null)
            throw new ArgumentNullException(nameof(sala));

        await _context.Salas.AddAsync(sala);
        return sala.IdSala;
    }

    /// <inheritdoc/>
    public Task UpdateAsync(Sala sala)
    {
        if (sala is null)
            throw new ArgumentNullException(nameof(sala));

        _context.Salas.Update(sala);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
