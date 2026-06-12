using SePrise.Domain.Aggregates;
using SePrise.Domain.ValueObjects;

namespace SePrise.Domain.Repositories;
public interface IAtencionRepository
{
    Task<AtencionAggregate?> GetByIdAsync(int id);
    Task<AtencionAggregate?> GetByTurnoAsync(int idTurno);
    Task<IEnumerable<AtencionAggregate>> GetByPacienteAsync(int idPaciente);
    Task<IEnumerable<AtencionAggregate>> GetByMedicoAsync(int idMedico);
    Task<IEnumerable<AtencionAggregate>> GetAtendiendoAsync(int idMedico);
    Task<IEnumerable<AtencionAggregate>> GetFinalizadasAsync(DateTime fechaDesde, DateTime fechaHasta);
    Task<IEnumerable<AtencionAggregate>> GetByEstadoAsync(EstadoAtencion estado);
    Task<IEnumerable<AtencionAggregate>> GetAllAsync();
    Task<int> AddAsync(AtencionAggregate atencion);
    Task UpdateAsync(AtencionAggregate atencion);
    Task SaveChangesAsync();
}


