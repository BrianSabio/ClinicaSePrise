using SePrise.Domain.Entities;

namespace SePrise.Domain.Repositories;

/// <summary>
/// Contrato del repositorio para la entidad asociativa <see cref="MedicoEspecialidad"/>.
/// Define las operaciones de acceso a datos para gestión de la relación N:N
/// entre médicos y especialidades.
/// Las implementaciones concretas residen en la capa de Infraestructura.
/// </summary>
public interface IMedicoEspecialidadRepository
{
    /// <summary>
    /// Busca la asociación específica entre un médico y una especialidad.
    /// </summary>
    /// <param name="idMedico">Identificador del médico.</param>
    /// <param name="idEspecialidad">Identificador de la especialidad.</param>
    /// <returns>La asociación encontrada, o <c>null</c> si no existe.</returns>
    Task<MedicoEspecialidad?> GetByIdsAsync(int idMedico, int idEspecialidad);

    /// <summary>
    /// Retorna todas las asociaciones de especialidades de un médico.
    /// Incluye la entidad <see cref="Especialidad"/> para acceder a sus datos.
    /// </summary>
    /// <param name="idMedico">Identificador del médico.</param>
    Task<IEnumerable<MedicoEspecialidad>> GetByMedicoAsync(int idMedico);

    /// <summary>
    /// Retorna todas las asociaciones de médicos para una especialidad.
    /// Incluye la entidad <see cref="Medico"/> para acceder a sus datos.
    /// </summary>
    /// <param name="idEspecialidad">Identificador de la especialidad.</param>
    Task<IEnumerable<MedicoEspecialidad>> GetByEspecialidadAsync(int idEspecialidad);

    /// <summary>
    /// Agrega una nueva asociación médico-especialidad al repositorio.
    /// No persiste los cambios hasta llamar a <see cref="SaveChangesAsync"/>.
    /// </summary>
    /// <param name="medicoEspecialidad">La entidad asociativa a agregar.</param>
    Task AddAsync(MedicoEspecialidad medicoEspecialidad);

    /// <summary>
    /// Elimina la asociación entre un médico y una especialidad.
    /// No persiste los cambios hasta llamar a <see cref="SaveChangesAsync"/>.
    /// </summary>
    /// <param name="idMedico">Identificador del médico.</param>
    /// <param name="idEspecialidad">Identificador de la especialidad.</param>
    Task RemoveAsync(int idMedico, int idEspecialidad);

    /// <summary>
    /// Persiste todos los cambios pendientes en la base de datos (Unit of Work).
    /// </summary>
    Task SaveChangesAsync();
}
