using SePrise.Domain.Entities;

namespace SePrise.Domain.Repositories;
public interface IEspecialidadRepository
{
    Task<Especialidad?> GetByIdAsync(int id);
    Task<Especialidad?> GetByNombreAsync(string nombre);
    Task<IEnumerable<Especialidad>> GetAllActivasAsync();
    Task<int> AddAsync(Especialidad especialidad);
    Task UpdateAsync(Especialidad especialidad);
    Task SaveChangesAsync();
}


