using SePrise.Domain.Entities;

namespace SePrise.Domain.Repositories;

/// <summary>
/// Contrato del repositorio para la entidad <see cref="Sala"/>.
/// Define las operaciones de acceso a datos para gestión de salas y consultorios.
/// Las implementaciones concretas residen en la capa de Infraestructura.
/// </summary>
public interface ISalaRepository
{
    /// <summary>
    /// Busca una sala por su identificador único.
    /// </summary>
    /// <param name="id">Identificador de la sala. Debe ser mayor a 0.</param>
    /// <returns>La sala encontrada, o <c>null</c> si no existe.</returns>
    Task<Sala?> GetByIdAsync(int id);

    /// <summary>
    /// Busca una sala por su número o código identificatorio.
    /// </summary>
    /// <param name="numero">Número o código de la sala.</param>
    /// <returns>La sala encontrada, o <c>null</c> si no existe.</returns>
    Task<Sala?> GetByNumeroAsync(string numero);

    /// <summary>
    /// Retorna todas las salas activas de un tipo específico, ordenadas por número.
    /// </summary>
    /// <param name="tipo">Tipo de sala a filtrar (<see cref="TipoSala"/>).</param>
    Task<IEnumerable<Sala>> GetAllActivasByTipoAsync(TipoSala tipo);

    /// <summary>
    /// Retorna todas las salas activas del sistema, ordenadas por número.
    /// </summary>
    Task<IEnumerable<Sala>> GetAllActivasAsync();

    /// <summary>
    /// Agrega una nueva sala al repositorio.
    /// No persiste los cambios hasta llamar a <see cref="SaveChangesAsync"/>.
    /// </summary>
    /// <param name="sala">La sala a agregar.</param>
    /// <returns>El identificador generado para la nueva sala.</returns>
    Task<int> AddAsync(Sala sala);

    /// <summary>
    /// Actualiza una sala existente en el repositorio.
    /// No persiste los cambios hasta llamar a <see cref="SaveChangesAsync"/>.
    /// </summary>
    /// <param name="sala">La sala con los datos actualizados.</param>
    Task UpdateAsync(Sala sala);

    /// <summary>
    /// Persiste todos los cambios pendientes en la base de datos (Unit of Work).
    /// </summary>
    Task SaveChangesAsync();
}
