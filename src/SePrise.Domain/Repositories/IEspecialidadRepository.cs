using SePrise.Domain.Entities;

namespace SePrise.Domain.Repositories;

/// <summary>
/// Contrato del repositorio para la entidad <see cref="Especialidad"/>.
/// Define las operaciones de acceso a datos para el catálogo de especialidades médicas.
/// Las implementaciones concretas residen en la capa de Infraestructura.
/// </summary>
public interface IEspecialidadRepository
{
    /// <summary>
    /// Busca una especialidad por su identificador único.
    /// </summary>
    /// <param name="id">Identificador de la especialidad. Debe ser mayor a 0.</param>
    /// <returns>La especialidad encontrada, o <c>null</c> si no existe.</returns>
    Task<Especialidad?> GetByIdAsync(int id);

    /// <summary>
    /// Busca una especialidad por su nombre (comparación insensible a mayúsculas/minúsculas).
    /// </summary>
    /// <param name="nombre">Nombre de la especialidad a buscar.</param>
    /// <returns>La especialidad encontrada, o <c>null</c> si no existe.</returns>
    Task<Especialidad?> GetByNombreAsync(string nombre);

    /// <summary>
    /// Retorna todas las especialidades activas, ordenadas por nombre.
    /// </summary>
    Task<IEnumerable<Especialidad>> GetAllActivasAsync();

    /// <summary>
    /// Agrega una nueva especialidad al repositorio.
    /// No persiste los cambios hasta llamar a <see cref="SaveChangesAsync"/>.
    /// </summary>
    /// <param name="especialidad">La especialidad a agregar.</param>
    /// <returns>El identificador generado para la nueva especialidad.</returns>
    Task<int> AddAsync(Especialidad especialidad);

    /// <summary>
    /// Actualiza una especialidad existente en el repositorio.
    /// No persiste los cambios hasta llamar a <see cref="SaveChangesAsync"/>.
    /// </summary>
    /// <param name="especialidad">La especialidad con los datos actualizados.</param>
    Task UpdateAsync(Especialidad especialidad);

    /// <summary>
    /// Persiste todos los cambios pendientes en la base de datos (Unit of Work).
    /// </summary>
    Task SaveChangesAsync();
}
