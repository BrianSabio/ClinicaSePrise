using SePrise.Domain.Entities;

namespace SePrise.Domain.Repositories;
public interface IMedicoEspecialidadRepository
{
    Task<MedicoEspecialidad?> GetByIdsAsync(int idMedico, int idEspecialidad);
    Task<IEnumerable<MedicoEspecialidad>> GetByMedicoAsync(int idMedico);
    Task<IEnumerable<MedicoEspecialidad>> GetByEspecialidadAsync(int idEspecialidad);
    Task AddAsync(MedicoEspecialidad medicoEspecialidad);
    Task RemoveAsync(int idMedico, int idEspecialidad);
    Task SaveChangesAsync();
}


