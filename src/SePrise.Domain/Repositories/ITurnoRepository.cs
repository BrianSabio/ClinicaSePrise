using SePrise.Domain.Aggregates;
using SePrise.Domain.ValueObjects;

namespace SePrise.Domain.Repositories;
public interface ITurnoRepository
{
    Task<TurnoAggregate?> GetByIdAsync(int id);
    Task<IEnumerable<TurnoAggregate>> GetByPacienteAsync(int idPaciente);
    Task<IEnumerable<TurnoAggregate>> GetByMedicoYFechaAsync(int idMedico, DateTime fecha);
    Task<IEnumerable<TurnoAggregate>> GetDisponiblesAsync(int idEspecialidad, DateTime fechaDesde, DateTime fechaHasta);
    Task<IEnumerable<TurnoAggregate>> GetByEstadoAsync(EstadoTurno estado);
    Task<IEnumerable<TurnoAggregate>> GetAllAsync();
    Task<int> AddAsync(TurnoAggregate turno);
    Task UpdateAsync(TurnoAggregate turno);
    Task SaveChangesAsync();
}


