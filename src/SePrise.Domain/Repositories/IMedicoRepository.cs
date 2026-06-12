using SePrise.Domain.Entities;

namespace SePrise.Domain.Repositories;
public interface IMedicoRepository
{
    Task<Medico?> GetByIdAsync(int id);
    Task<Medico?> GetByNumeroMatriculaAsync(string numeroMatricula);
    Task<IEnumerable<Medico>> GetAllActivosAsync();
    Task<IEnumerable<Especialidad>> GetEspecialidadesByMedicoAsync(int idMedico);
    Task<int> AddAsync(Medico medico);
    Task UpdateAsync(Medico medico);
    Task SaveChangesAsync();
}


