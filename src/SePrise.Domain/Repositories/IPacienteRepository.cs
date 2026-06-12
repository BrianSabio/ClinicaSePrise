using SePrise.Domain.Entities;
using SePrise.Domain.ValueObjects;

namespace SePrise.Domain.Repositories;
public interface IPacienteRepository
{
    Task<Paciente?> GetByIdAsync(int id);
    Task<Paciente?> GetByDNIAsync(Dni dni);
    Task<IEnumerable<Paciente>> GetAllActivosAsync();
    Task<bool> ExistsByDNIAsync(Dni dni);
    Task<int> AddAsync(Paciente paciente);
    Task UpdateAsync(Paciente paciente);
    Task SaveChangesAsync();
}


