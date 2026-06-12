using SePrise.Domain.Entities;

namespace SePrise.Domain.Repositories;

/// <summary>
/// Contrato del repositorio para la entidad <see cref="Medico"/>.
/// Define las operaciones de acceso a datos para gestión de médicos y sus especialidades.
/// Las implementaciones concretas residen en la capa de Infraestructura.
/// </summary>
public interface IMedicoRepository
{
    /// <summary>
    /// Busca un médico por su identificador único.
    /// </summary>
    /// <param name="id">Identificador del médico. Debe ser mayor a 0.</param>
    /// <returns>El médico encontrado, o <c>null</c> si no existe.</returns>
    Task<Medico?> GetByIdAsync(int id);

    /// <summary>
    /// Busca un médico por su número de matrícula profesional.
    /// </summary>
    /// <param name="numeroMatricula">Número de matrícula único del médico.</param>
    /// <returns>El médico encontrado, o <c>null</c> si no existe.</returns>
    Task<Medico?> GetByNumeroMatriculaAsync(string numeroMatricula);

    /// <summary>
    /// Retorna todos los médicos activos, ordenados por Apellido y luego por Nombre.
    /// </summary>
    Task<IEnumerable<Medico>> GetAllActivosAsync();

    /// <summary>
    /// Retorna todas las especialidades asociadas a un médico.
    /// </summary>
    /// <param name="idMedico">Identificador del médico.</param>
    /// <returns>Colección de especialidades del médico. Vacía si no tiene ninguna.</returns>
    Task<IEnumerable<Especialidad>> GetEspecialidadesByMedicoAsync(int idMedico);

    /// <summary>
    /// Agrega un nuevo médico al repositorio.
    /// No persiste los cambios hasta llamar a <see cref="SaveChangesAsync"/>.
    /// </summary>
    /// <param name="medico">El médico a agregar.</param>
    /// <returns>El identificador generado para el nuevo médico.</returns>
    Task<int> AddAsync(Medico medico);

    /// <summary>
    /// Actualiza un médico existente en el repositorio.
    /// No persiste los cambios hasta llamar a <see cref="SaveChangesAsync"/>.
    /// </summary>
    /// <param name="medico">El médico con los datos actualizados.</param>
    Task UpdateAsync(Medico medico);

    /// <summary>
    /// Persiste todos los cambios pendientes en la base de datos (Unit of Work).
    /// </summary>
    Task SaveChangesAsync();
}
