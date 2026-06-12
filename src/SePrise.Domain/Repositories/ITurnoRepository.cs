using SePrise.Domain.Aggregates;
using SePrise.Domain.ValueObjects;

namespace SePrise.Domain.Repositories;

/// <summary>
/// Contrato del repositorio para el agregado <see cref="TurnoAggregate"/>.
/// Define las operaciones de acceso a datos para gestión del ciclo de vida de los turnos.
/// Las implementaciones concretas residen en la capa de Infraestructura.
/// </summary>
public interface ITurnoRepository
{
    /// <summary>
    /// Busca un turno por su identificador único, cargando todas las relaciones necesarias.
    /// </summary>
    /// <param name="id">Identificador del turno. Debe ser mayor a 0.</param>
    /// <returns>El turno con sus relaciones cargadas, o <c>null</c> si no existe.</returns>
    Task<TurnoAggregate?> GetByIdAsync(int id);

    /// <summary>
    /// Retorna todos los turnos de un paciente, ordenados por fecha descendente.
    /// Incluye médico y especialidad para mostrar al paciente su historial.
    /// </summary>
    /// <param name="idPaciente">Identificador del paciente.</param>
    Task<IEnumerable<TurnoAggregate>> GetByPacienteAsync(int idPaciente);

    /// <summary>
    /// Retorna los turnos de un médico en una fecha específica, ordenados por hora de inicio.
    /// Útil para mostrar la agenda diaria del médico.
    /// </summary>
    /// <param name="idMedico">Identificador del médico.</param>
    /// <param name="fecha">Fecha de la agenda a consultar.</param>
    Task<IEnumerable<TurnoAggregate>> GetByMedicoYFechaAsync(int idMedico, DateTime fecha);

    /// <summary>
    /// Retorna turnos disponibles (estado Reservado) para una especialidad en un rango de fechas.
    /// Útil para mostrar disponibilidad al paciente al agendar un nuevo turno.
    /// </summary>
    /// <param name="idEspecialidad">Identificador de la especialidad.</param>
    /// <param name="fechaDesde">Fecha inicial del rango de búsqueda.</param>
    /// <param name="fechaHasta">Fecha final del rango de búsqueda.</param>
    Task<IEnumerable<TurnoAggregate>> GetDisponiblesAsync(int idEspecialidad, DateTime fechaDesde, DateTime fechaHasta);

    /// <summary>
    /// Retorna todos los turnos en un estado específico.
    /// Útil para reportes y procesos de cierre del día.
    /// </summary>
    /// <param name="estado">Estado del turno a filtrar.</param>
    Task<IEnumerable<TurnoAggregate>> GetByEstadoAsync(EstadoTurno estado);

    /// <summary>
    /// Retorna todos los turnos del sistema, ordenados por fecha descendente.
    /// Útil para el listado general de la recepción.
    /// </summary>
    Task<IEnumerable<TurnoAggregate>> GetAllAsync();

    /// <summary>
    /// Agrega un nuevo turno al repositorio.
    /// No persiste los cambios hasta llamar a <see cref="SaveChangesAsync"/>.
    /// </summary>
    /// <param name="turno">El agregado turno a agregar.</param>
    /// <returns>El identificador generado para el nuevo turno.</returns>
    Task<int> AddAsync(TurnoAggregate turno);

    /// <summary>
    /// Actualiza un turno existente en el repositorio.
    /// No persiste los cambios hasta llamar a <see cref="SaveChangesAsync"/>.
    /// </summary>
    /// <param name="turno">El agregado turno con los datos actualizados.</param>
    Task UpdateAsync(TurnoAggregate turno);

    /// <summary>
    /// Persiste todos los cambios pendientes en la base de datos (Unit of Work).
    /// </summary>
    Task SaveChangesAsync();
}
