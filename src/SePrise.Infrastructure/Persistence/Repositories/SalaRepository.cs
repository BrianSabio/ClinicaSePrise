using Microsoft.EntityFrameworkCore;
using SePrise.Domain.Entities;
using SePrise.Domain.Repositories;

namespace SePrise.Infrastructure.Persistence.Repositories;
public class SalaRepository : ISalaRepository
{
    private readonly SePriseDbContext _context;
    public SalaRepository(SePriseDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    public async Task<Sala?> GetByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("El ID de la sala debe ser mayor a 0.", nameof(id));

        return await _context.Salas
            .FirstOrDefaultAsync(s => s.IdSala == id);
    }
    public async Task<Sala?> GetByNumeroAsync(string numero)
    {
        if (string.IsNullOrWhiteSpace(numero))
            throw new ArgumentException("El número de sala no puede ser nulo ni vacío.", nameof(numero));

        return await _context.Salas
            .FirstOrDefaultAsync(s => s.Numero == numero.Trim());
    }
    public async Task<IEnumerable<Sala>> GetAllActivasByTipoAsync(TipoSala tipo)
    {
        // Filtrar por tipo de sala (enum almacenado como int en BD)
        return await _context.Salas
            .Where(s => s.Activo && s.TipoSala == tipo)
            .OrderBy(s => s.Numero)
            .ToListAsync();
    }
    public async Task<IEnumerable<Sala>> GetAllActivasAsync()
    {
        return await _context.Salas
            .Where(s => s.Activo)
            .OrderBy(s => s.Numero)
            .ToListAsync();
    }
    public async Task<int> AddAsync(Sala sala)
    {
        if (sala is null)
            throw new ArgumentNullException(nameof(sala));

        await _context.Salas.AddAsync(sala);
        return sala.IdSala;
    }
    public Task UpdateAsync(Sala sala)
    {
        if (sala is null)
            throw new ArgumentNullException(nameof(sala));

        _context.Salas.Update(sala);
        return Task.CompletedTask;
    }
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}


