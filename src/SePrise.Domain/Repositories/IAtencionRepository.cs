using SePrise.Domain.Aggregates;
using SePrise.Domain.ValueObjects;

namespace SePrise.Domain.Repositories;

/// <summary>
/// Contrato del repositorio para el agregado <see cref="AtencionAggregate"/>.
/// Define las operaciones de acceso a datos para gestión del ciclo de vida de las atenciones.
/// Las implementaciones concretas residen en la capa de Infraestructura.
/// </summary>
public interface IAtencionRepository
{
    /// <summary>
    /// Busca una atención por su identificador único, cargando todas las relaciones necesarias.
    /// </summary>
    /// <param name="id">Identificador de la atención. Debe ser mayor a 0.</param>
    /// <returns>La atención con sus relaciones cargadas, o <c>null</c> si no existe.</returns>
    Task<AtencionAggregate?> GetByIdAsync(int id);

    /// <summary>
    /// Busca la atención asociada a un turno específico.
    /// Retorna <c>null</c> si el turno aún no tiene atención (relación 0..1).
    /// </summary>
    /// <param name="idTurno">Identificador del turno.</param>
    Task<AtencionAggregate?> GetByTurnoAsync(int idTurno);

    /// <summary>
    /// Retorna todas las atenciones de un paciente, ordenadas por fecha de acreditación descendente.
    /// Incluye turno, médico y datos de la atención para historial clínico.
    /// </summary>
    /// <param name="idPaciente">Identificador del paciente.</param>
    Task<IEnumerable<AtencionAggregate>> GetByPacienteAsync(int idPaciente);

    /// <summary>
    /// Retorna todas las atenciones de un médico, ordenadas por fecha de acreditación descendente.
    /// </summary>
    /// <param name="idMedico">Identificador del médico.</param>
    Task<IEnumerable<AtencionAggregate>> GetByMedicoAsync(int idMedico);

    /// <summary>
    /// Retorna las atenciones actualmente en progreso para un médico.
    /// Útil para mostrar la cola de pacientes en el consultorio.
    /// </summary>
    /// <param name="idMedico">Identificador del médico.</param>
    Task<IEnumerable<AtencionAggregate>> GetAtendiendoAsync(int idMedico);

    /// <summary>
    /// Retorna las atenciones finalizadas en un rango de fechas.
    /// Útil para reportes de productividad o facturación.
    /// </summary>
    /// <param name="fechaDesde">Fecha inicial del rango.</param>
    /// <param name="fechaHasta">Fecha final del rango.</param>
    Task<IEnumerable<AtencionAggregate>> GetFinalizadasAsync(DateTime fechaDesde, DateTime fechaHasta);

    /// <summary>
    /// Retorna todas las atenciones en un estado específico.
    /// </summary>
    /// <param name="estado">Estado de la atención a filtrar.</param>
    Task<IEnumerable<AtencionAggregate>> GetByEstadoAsync(EstadoAtencion estado);

    /// <summary>
    /// Retorna todas las atenciones sin filtro.
    /// </summary>
    Task<IEnumerable<AtencionAggregate>> GetAllAsync();

    /// <summary>
    /// Agrega una nueva atención al repositorio.
    /// No persiste los cambios hasta llamar a <see cref="SaveChangesAsync"/>.
    /// </summary>
    /// <param name="atencion">El agregado atención a agregar.</param>
    /// <returns>El identificador generado para la nueva atención.</returns>
    Task<int> AddAsync(AtencionAggregate atencion);

    /// <summary>
    /// Actualiza una atención existente en el repositorio.
    /// No persiste los cambios hasta llamar a <see cref="SaveChangesAsync"/>.
    /// </summary>
    /// <param name="atencion">El agregado atención con los datos actualizados.</param>
    Task UpdateAsync(AtencionAggregate atencion);

    /// <summary>
    /// Persiste todos los cambios pendientes en la base de datos (Unit of Work).
    /// </summary>
    Task SaveChangesAsync();
}
