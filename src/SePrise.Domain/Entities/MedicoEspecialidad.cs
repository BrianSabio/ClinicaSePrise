namespace SePrise.Domain.Entities;

/// <summary>
/// Entidad asociativa que representa la relación N:N entre un <see cref="Medico"/>
/// y una <see cref="Especialidad"/>. Permite que un médico tenga múltiples especialidades
/// y que una especialidad sea ejercida por múltiples médicos.
/// La clave primaria es compuesta: (IdMedico, IdEspecialidad), configurada en EF Core.
/// </summary>
public class MedicoEspecialidad : Entity
{
    /// <summary>
    /// Parte 1 de la clave primaria compuesta.
    /// Clave foránea que referencia al <see cref="Medico"/> propietario de la relación.
    /// </summary>
    public int IdMedico { get; private set; }

    /// <summary>
    /// Parte 2 de la clave primaria compuesta.
    /// Clave foránea que referencia a la <see cref="Especialidad"/> asignada al médico.
    /// </summary>
    public int IdEspecialidad { get; private set; }

    /// <summary>
    /// Propiedad de navegación al médico de la relación.
    /// Marcada como <c>virtual</c> para habilitar el lazy loading de Entity Framework Core.
    /// </summary>
    public virtual Medico Medico { get; private set; } = null!;

    /// <summary>
    /// Propiedad de navegación a la especialidad de la relación.
    /// Marcada como <c>virtual</c> para habilitar el lazy loading de Entity Framework Core.
    /// </summary>
    public virtual Especialidad Especialidad { get; private set; } = null!;

    /// <summary>
    /// Constructor privado sin parámetros requerido por Entity Framework Core
    /// para la materialización de entidades desde la base de datos.
    /// </summary>
    private MedicoEspecialidad() { }

    /// <summary>
    /// Factory method para crear una nueva asociación entre un médico y una especialidad.
    /// Solo valida que los identificadores sean positivos — la existencia de los registros
    /// es responsabilidad del repositorio o servicio de aplicación.
    /// </summary>
    /// <param name="idMedico">
    /// Identificador del médico. Debe ser un entero positivo mayor a cero.
    /// </param>
    /// <param name="idEspecialidad">
    /// Identificador de la especialidad. Debe ser un entero positivo mayor a cero.
    /// </param>
    /// <returns>
    /// Una instancia válida de <see cref="MedicoEspecialidad"/> con los timestamps de auditoría.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Se lanza si <paramref name="idMedico"/> o <paramref name="idEspecialidad"/> son menores o iguales a cero.
    /// </exception>
    public static MedicoEspecialidad Crear(int idMedico, int idEspecialidad)
    {
        // Validar que el ID del médico sea un valor positivo válido
        if (idMedico <= 0)
            throw new ArgumentException(
                $"El IdMedico debe ser un valor positivo mayor a cero. " +
                $"Se recibió: {idMedico}.",
                nameof(idMedico));

        // Validar que el ID de la especialidad sea un valor positivo válido
        if (idEspecialidad <= 0)
            throw new ArgumentException(
                $"El IdEspecialidad debe ser un valor positivo mayor a cero. " +
                $"Se recibió: {idEspecialidad}.",
                nameof(idEspecialidad));

        var ahora = DateTime.UtcNow;

        return new MedicoEspecialidad
        {
            IdMedico = idMedico,
            IdEspecialidad = idEspecialidad,
            FechaCreacion = ahora,
            FechaUltimaModificacion = ahora
        };
    }

    /// <summary>
    /// Compara esta instancia con otro objeto usando la identidad de negocio.
    /// Dos instancias de <see cref="MedicoEspecialidad"/> son iguales si tienen
    /// el mismo <see cref="IdMedico"/> e <see cref="IdEspecialidad"/>.
    /// </summary>
    /// <param name="obj">El objeto a comparar con la instancia actual.</param>
    /// <returns>
    /// <c>true</c> si ambos representan la misma relación médico-especialidad;
    /// <c>false</c> en caso contrario.
    /// </returns>
    public override bool Equals(object? obj)
    {
        // Si el objeto es null o no es del tipo MedicoEspecialidad, no son iguales
        if (obj is not MedicoEspecialidad otra)
            return false;

        // La igualdad se determina por la clave de negocio compuesta
        return IdMedico == otra.IdMedico && IdEspecialidad == otra.IdEspecialidad;
    }

    /// <summary>
    /// Genera un código hash basado en la clave de negocio compuesta (IdMedico, IdEspecialidad).
    /// Es consistente con <see cref="Equals"/>: dos instancias iguales producen el mismo hash.
    /// </summary>
    /// <returns>El código hash combinado de <see cref="IdMedico"/> e <see cref="IdEspecialidad"/>.</returns>
    public override int GetHashCode() =>
        HashCode.Combine(IdMedico, IdEspecialidad);

    /// <summary>
    /// Operador de igualdad entre dos instancias de <see cref="MedicoEspecialidad"/>.
    /// </summary>
    public static bool operator ==(MedicoEspecialidad? izquierda, MedicoEspecialidad? derecha) =>
        izquierda?.Equals(derecha) ?? derecha is null;

    /// <summary>
    /// Operador de desigualdad entre dos instancias de <see cref="MedicoEspecialidad"/>.
    /// </summary>
    public static bool operator !=(MedicoEspecialidad? izquierda, MedicoEspecialidad? derecha) =>
        !(izquierda == derecha);
}
