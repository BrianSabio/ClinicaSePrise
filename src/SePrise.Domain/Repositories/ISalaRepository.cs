using SePrise.Domain.Entities;

namespace SePrise.Domain.Repositories;
public interface ISalaRepository
{
    Task<Sala?> GetByIdAsync(int id);
    Task<Sala?> GetByNumeroAsync(string numero);
    Task<IEnumerable<Sala>> GetAllActivasByTipoAsync(TipoSala tipo);
    Task<IEnumerable<Sala>> GetAllActivasAsync();
    Task<int> AddAsync(Sala sala);
    Task UpdateAsync(Sala sala);
    Task SaveChangesAsync();
}


