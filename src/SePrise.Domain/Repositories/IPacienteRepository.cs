using SePrise.Domain.Entities;
using SePrise.Domain.ValueObjects;

namespace SePrise.Domain.Repositories;

/// <summary>
/// Contrato del repositorio para la entidad <see cref="Paciente"/>.
/// Define las operaciones de acceso a datos para gestión de pacientes.
/// Las implementaciones concretas residen en la capa de Infraestructura.
/// </summary>
public interface IPacienteRepository
{
    /// <summary>
    /// Busca un paciente por su identificador único.
    /// </summary>
    /// <param name="id">Identificador del paciente. Debe ser mayor a 0.</param>
    /// <returns>El paciente encontrado, o <c>null</c> si no existe.</returns>
    Task<Paciente?> GetByIdAsync(int id);

    /// <summary>
    /// Busca un paciente por su DNI (Value Object).
    /// Útil para acreditación en recepción.
    /// </summary>
    /// <param name="dni">Value Object DNI del paciente.</param>
    /// <returns>El paciente encontrado, o <c>null</c> si no existe.</returns>
    Task<Paciente?> GetByDNIAsync(Dni dni);

    /// <summary>
    /// Retorna todos los pacientes activos en el sistema,
    /// ordenados por Apellido y luego por Nombre.
    /// </summary>
    Task<IEnumerable<Paciente>> GetAllActivosAsync();

    /// <summary>
    /// Verifica si ya existe un paciente registrado con el DNI especificado.
    /// Útil para validar unicidad antes de registrar un nuevo paciente.
    /// </summary>
    /// <param name="dni">Value Object DNI a verificar.</param>
    /// <returns><c>true</c> si el DNI ya existe; <c>false</c> en caso contrario.</returns>
    Task<bool> ExistsByDNIAsync(Dni dni);

    /// <summary>
    /// Agrega un nuevo paciente al repositorio.
    /// No persiste los cambios hasta llamar a <see cref="SaveChangesAsync"/>.
    /// </summary>
    /// <param name="paciente">La entidad paciente a agregar.</param>
    /// <returns>El identificador generado para el nuevo paciente.</returns>
    Task<int> AddAsync(Paciente paciente);

    /// <summary>
    /// Actualiza un paciente existente en el repositorio.
    /// No persiste los cambios hasta llamar a <see cref="SaveChangesAsync"/>.
    /// </summary>
    /// <param name="paciente">La entidad paciente con los datos actualizados.</param>
    Task UpdateAsync(Paciente paciente);

    /// <summary>
    /// Persiste todos los cambios pendientes en la base de datos (Unit of Work).
    /// </summary>
    Task SaveChangesAsync();
}
