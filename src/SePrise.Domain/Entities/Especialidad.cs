namespace SePrise.Domain.Entities;

/// <summary>
/// Entidad que representa una especialidad médica disponible en la clínica.
/// Funciona como catálogo de especialidades y define la duración estándar de consulta.
/// </summary>
public class Especialidad : Entity
{
    /// <summary>
    /// Identificador único de la especialidad (clave primaria, generado por la base de datos).
    /// </summary>
    public int IdEspecialidad { get; private set; }

    /// <summary>
    /// Nombre de la especialidad médica (por ejemplo: "Cardiología", "Odontología").
    /// Es único en la base de datos. Longitud máxima: 100 caracteres.
    /// </summary>
    public string Nombre { get; private set; } = string.Empty;

    /// <summary>
    /// Descripción extendida de la especialidad y su alcance clínico.
    /// Puede ser nula si no se especifica. Longitud máxima: 500 caracteres.
    /// </summary>
    public string? Descripcion { get; private set; }

    /// <summary>
    /// Duración estándar en minutos de una consulta para esta especialidad.
    /// Valor mínimo: 15 minutos. Valor por defecto: 30 minutos.
    /// </summary>
    public int DuracionMinutos { get; private set; }

    /// <summary>
    /// Indica si la especialidad permite registrar más de un turno por día
    /// para el mismo paciente. Por defecto es <c>false</c>.
    /// </summary>
    public bool PermiteMultiplesTurnos { get; private set; }

    /// <summary>
    /// Indica si la especialidad está activa en el sistema.
    /// Las especialidades inactivas no se eliminan físicamente (soft delete).
    /// </summary>
    public bool Activo { get; private set; }

    /// <summary>
    /// Constructor privado sin parámetros requerido por Entity Framework Core
    /// para la materialización de entidades desde la base de datos.
    /// </summary>
    private Especialidad() { }

    /// <summary>
    /// Factory method para crear una nueva especialidad médica con los valores validados.
    /// Garantiza que la especialidad se construya en un estado consistente con las
    /// invariantes del dominio.
    /// </summary>
    /// <param name="nombre">
    /// Nombre de la especialidad. No puede ser nulo ni vacío. Máximo 100 caracteres.
    /// </param>
    /// <param name="descripcion">
    /// Descripción opcional de la especialidad. Máximo 500 caracteres.
    /// </param>
    /// <param name="duracionMinutos">
    /// Duración en minutos de una consulta estándar. Mínimo 15 minutos. Por defecto: 30.
    /// </param>
    /// <param name="permiteMultiplesTurnos">
    /// Indica si el mismo paciente puede tener más de un turno por día. Por defecto: false.
    /// </param>
    /// <returns>Una instancia válida de <see cref="Especialidad"/> con estado activo.</returns>
    /// <exception cref="ArgumentException">
    /// Se lanza si el nombre es nulo/vacío, excede los 100 caracteres,
    /// o si la duración es menor a 15 minutos.
    /// </exception>
    public static Especialidad Crear(
        string nombre,
        string? descripcion = null,
        int duracionMinutos = 30,
        bool permiteMultiplesTurnos = false)
    {
        // Validar que el nombre no sea nulo ni vacío
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException(
                "El nombre de la especialidad no puede ser nulo ni estar vacío.",
                nameof(nombre));

        // Validar longitud máxima del nombre
        if (nombre.Trim().Length > 100)
            throw new ArgumentException(
                $"El nombre de la especialidad no puede exceder 100 caracteres. " +
                $"Se recibieron {nombre.Trim().Length} caracteres.",
                nameof(nombre));

        // Validar que la descripción no supere el límite si fue proporcionada
        if (descripcion is not null && descripcion.Length > 500)
            throw new ArgumentException(
                $"La descripción no puede exceder 500 caracteres. " +
                $"Se recibieron {descripcion.Length} caracteres.",
                nameof(descripcion));

        // Validar duración mínima de consulta
        if (duracionMinutos < 15)
            throw new ArgumentException(
                $"La duración mínima de una consulta es 15 minutos. " +
                $"Se recibieron {duracionMinutos} minutos.",
                nameof(duracionMinutos));

        return new Especialidad
        {
            Nombre = nombre.Trim(),
            Descripcion = descripcion,
            DuracionMinutos = duracionMinutos,
            PermiteMultiplesTurnos = permiteMultiplesTurnos,
            Activo = true,
            FechaCreacion = DateTime.UtcNow,
            FechaUltimaModificacion = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Marca la especialidad como inactiva en el sistema (soft delete).
    /// Las especialidades inactivas no estarán disponibles para nuevos turnos.
    /// Actualiza automáticamente <see cref="Entity.FechaUltimaModificacion"/>.
    /// </summary>
    public void Desactivar()
    {
        Activo = false;
        ActualizarFechaModificacion();
    }
}
