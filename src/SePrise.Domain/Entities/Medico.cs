namespace SePrise.Domain.Entities;

/// <summary>
/// Entidad que representa un médico profesional registrado en la clínica.
/// Contiene la información de identificación, contacto y estado activo del profesional.
/// </summary>
public class Medico : Entity
{
    /// <summary>
    /// Identificador único del médico (clave primaria, generado por la base de datos).
    /// </summary>
    public int IdMedico { get; private set; }

    /// <summary>
    /// Número de matrícula médica profesional. Es único en el sistema.
    /// Longitud máxima: 50 caracteres. No puede ser nulo ni vacío.
    /// </summary>
    public string NumeroMatricula { get; private set; } = string.Empty;

    /// <summary>
    /// Nombre del médico. Longitud máxima: 100 caracteres.
    /// </summary>
    public string Nombre { get; private set; } = string.Empty;

    /// <summary>
    /// Apellido del médico. Longitud máxima: 100 caracteres.
    /// </summary>
    public string Apellido { get; private set; } = string.Empty;

    /// <summary>
    /// Dirección de correo electrónico de contacto del médico.
    /// Puede ser nulo. Longitud máxima: 100 caracteres.
    /// </summary>
    public string? Email { get; private set; }

    /// <summary>
    /// Número de teléfono de contacto del médico.
    /// Puede ser nulo. Longitud máxima: 20 caracteres.
    /// </summary>
    public string? Telefono { get; private set; }

    /// <summary>
    /// Indica si el médico está activo y disponible para recibir turnos.
    /// Las bajas son lógicas (soft delete). Por defecto: <c>true</c>.
    /// </summary>
    public bool Activo { get; private set; }

    /// <summary>
    /// Fecha y hora (UTC) en que el médico fue dado de alta en el sistema.
    /// </summary>
    public DateTime FechaAlta { get; private set; }

    /// <summary>
    /// Constructor privado sin parámetros requerido por Entity Framework Core
    /// para la materialización de entidades desde la base de datos.
    /// </summary>
    private Medico() { }

    /// <summary>
    /// Factory method para crear un nuevo médico con los datos validados.
    /// Garantiza que el médico se construya en un estado consistente con las
    /// invariantes del dominio.
    /// </summary>
    /// <param name="numeroMatricula">
    /// Número de matrícula profesional. No puede ser nulo ni vacío. Máximo 50 caracteres.
    /// </param>
    /// <param name="nombre">
    /// Nombre del médico. No puede ser nulo ni vacío. Máximo 100 caracteres.
    /// </param>
    /// <param name="apellido">
    /// Apellido del médico. No puede ser nulo ni vacío. Máximo 100 caracteres.
    /// </param>
    /// <param name="email">
    /// Email de contacto opcional. Máximo 100 caracteres si se proporciona.
    /// </param>
    /// <param name="telefono">
    /// Teléfono de contacto opcional. Máximo 20 caracteres si se proporciona.
    /// </param>
    /// <returns>Una instancia válida de <see cref="Medico"/> con estado activo.</returns>
    /// <exception cref="ArgumentException">
    /// Se lanza si la matrícula, nombre o apellido son nulos/vacíos o exceden su longitud máxima.
    /// </exception>
    public static Medico Crear(
        string numeroMatricula,
        string nombre,
        string apellido,
        string? email = null,
        string? telefono = null)
    {
        // Validar número de matrícula
        if (string.IsNullOrWhiteSpace(numeroMatricula))
            throw new ArgumentException(
                "El número de matrícula no puede ser nulo ni estar vacío.",
                nameof(numeroMatricula));

        if (numeroMatricula.Trim().Length > 50)
            throw new ArgumentException(
                $"El número de matrícula no puede exceder 50 caracteres. " +
                $"Se recibieron {numeroMatricula.Trim().Length} caracteres.",
                nameof(numeroMatricula));

        // Validar nombre
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException(
                "El nombre del médico no puede ser nulo ni estar vacío.",
                nameof(nombre));

        if (nombre.Trim().Length > 100)
            throw new ArgumentException(
                $"El nombre no puede exceder 100 caracteres. " +
                $"Se recibieron {nombre.Trim().Length} caracteres.",
                nameof(nombre));

        // Validar apellido
        if (string.IsNullOrWhiteSpace(apellido))
            throw new ArgumentException(
                "El apellido del médico no puede ser nulo ni estar vacío.",
                nameof(apellido));

        if (apellido.Trim().Length > 100)
            throw new ArgumentException(
                $"El apellido no puede exceder 100 caracteres. " +
                $"Se recibieron {apellido.Trim().Length} caracteres.",
                nameof(apellido));

        // Validar email si se proporcionó
        if (email is not null && email.Length > 100)
            throw new ArgumentException(
                $"El email no puede exceder 100 caracteres. " +
                $"Se recibieron {email.Length} caracteres.",
                nameof(email));

        // Validar teléfono si se proporcionó
        if (telefono is not null && telefono.Length > 20)
            throw new ArgumentException(
                $"El teléfono no puede exceder 20 caracteres. " +
                $"Se recibieron {telefono.Length} caracteres.",
                nameof(telefono));

        // Capturar el momento exacto de alta para consistencia
        var ahora = DateTime.UtcNow;

        return new Medico
        {
            NumeroMatricula = numeroMatricula.Trim(),
            Nombre = nombre.Trim(),
            Apellido = apellido.Trim(),
            Email = email,
            Telefono = telefono,
            Activo = true,
            FechaAlta = ahora,
            FechaCreacion = ahora,
            FechaUltimaModificacion = ahora
        };
    }

    /// <summary>
    /// Marca al médico como inactivo en el sistema (soft delete).
    /// Los médicos inactivos no estarán disponibles para nuevos turnos.
    /// Actualiza automáticamente <see cref="Entity.FechaUltimaModificacion"/>.
    /// </summary>
    public void Desactivar()
    {
        Activo = false;
        ActualizarFechaModificacion();
    }
}
