namespace SePrise.Domain.Entities;

/// <summary>
/// Clase base abstracta para todas las entidades del dominio SePrise.
/// Provee propiedades de auditoría temporal y el método para registrar modificaciones.
/// Todas las entidades y agregados deben heredar de esta clase.
/// </summary>
public abstract class Entity
{
    /// <summary>
    /// Fecha y hora (UTC) en que el registro fue creado por primera vez en el sistema.
    /// Solo se asigna una vez durante la construcción de la entidad.
    /// </summary>
    public DateTime FechaCreacion { get; protected set; } = DateTime.UtcNow;

    /// <summary>
    /// Fecha y hora (UTC) de la última modificación del registro.
    /// Se actualiza automáticamente cada vez que la entidad cambia de estado.
    /// </summary>
    public DateTime FechaUltimaModificacion { get; protected set; } = DateTime.UtcNow;

    /// <summary>
    /// Constructor protegido sin parámetros, disponible para las clases derivadas.
    /// Entity Framework Core requiere un constructor sin parámetros para materializar entidades.
    /// </summary>
    protected Entity() { }

    /// <summary>
    /// Actualiza la propiedad <see cref="FechaUltimaModificacion"/> al momento actual (UTC).
    /// Debe invocarse en cada método de la entidad o agregado que modifique su estado interno,
    /// garantizando trazabilidad de cambios.
    /// </summary>
    protected void ActualizarFechaModificacion()
    {
        FechaUltimaModificacion = DateTime.UtcNow;
    }
}
