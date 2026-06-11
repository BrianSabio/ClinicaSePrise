using SePrise.Domain.ValueObjects;

namespace SePrise.Domain.Entities;

/// <summary>
/// Entidad que representa un paciente registrado en el sistema de la clínica.
/// Contiene toda la información demográfica, de contacto y estado activo del paciente.
/// El DNI se gestiona mediante el Value Object <see cref="Dni"/> para garantizar su validez.
/// </summary>
public class Paciente : Entity
{
    /// <summary>
    /// Identificador único del paciente (clave primaria, generado por la base de datos).
    /// </summary>
    public int IdPaciente { get; private set; }

    /// <summary>
    /// Documento Nacional de Identidad del paciente, encapsulado en el Value Object <see cref="Dni"/>.
    /// Es único en el sistema: no puede haber dos pacientes con el mismo DNI.
    /// </summary>
    public Dni DNI { get; private set; } = null!;

    /// <summary>
    /// Nombre(s) del paciente. Obligatorio. Longitud máxima: 100 caracteres.
    /// </summary>
    public string Nombre { get; private set; } = string.Empty;

    /// <summary>
    /// Apellido(s) del paciente. Obligatorio. Longitud máxima: 100 caracteres.
    /// </summary>
    public string Apellido { get; private set; } = string.Empty;

    /// <summary>
    /// Fecha de nacimiento del paciente (solo fecha, sin componente horario).
    /// Debe ser anterior a la fecha actual.
    /// </summary>
    public DateTime FechaNacimiento { get; private set; }

    /// <summary>
    /// Género del paciente. Valores válidos: 'M' (Masculino), 'F' (Femenino), 'O' (Otro).
    /// </summary>
    public char Genero { get; private set; }

    /// <summary>
    /// Dirección de correo electrónico del paciente.
    /// Opcional. Longitud máxima: 100 caracteres.
    /// </summary>
    public string? Email { get; private set; }

    /// <summary>
    /// Número de teléfono de contacto del paciente.
    /// Opcional. Longitud máxima: 20 caracteres.
    /// </summary>
    public string? Telefono { get; private set; }

    /// <summary>
    /// Dirección de domicilio del paciente.
    /// Opcional. Longitud máxima: 200 caracteres.
    /// </summary>
    public string? Direccion { get; private set; }

    /// <summary>
    /// Ciudad de residencia del paciente.
    /// Opcional. Longitud máxima: 50 caracteres.
    /// </summary>
    public string? Ciudad { get; private set; }

    /// <summary>
    /// Provincia o estado de residencia del paciente.
    /// Opcional. Longitud máxima: 50 caracteres.
    /// </summary>
    public string? Provincia { get; private set; }

    /// <summary>
    /// Código postal del domicilio del paciente.
    /// Opcional. Longitud máxima: 10 caracteres.
    /// </summary>
    public string? CodigoPostal { get; private set; }

    /// <summary>
    /// Indica si el paciente está activo en el sistema.
    /// Los pacientes inactivos no se eliminan físicamente (soft delete). Por defecto: <c>true</c>.
    /// </summary>
    public bool Activo { get; private set; }

    /// <summary>
    /// Constructor privado sin parámetros requerido por Entity Framework Core
    /// para la materialización de entidades desde la base de datos.
    /// </summary>
    private Paciente() { }

    /// <summary>
    /// Factory method para crear un nuevo paciente con los datos validados.
    /// Garantiza que el paciente se construya en un estado consistente con las
    /// invariantes del dominio.
    /// </summary>
    /// <param name="dni">
    /// Value Object DNI ya validado. No puede ser nulo.
    /// </param>
    /// <param name="nombre">
    /// Nombre del paciente. No puede ser nulo ni vacío. Máximo 100 caracteres.
    /// </param>
    /// <param name="apellido">
    /// Apellido del paciente. No puede ser nulo ni vacío. Máximo 100 caracteres.
    /// </param>
    /// <param name="fechaNacimiento">
    /// Fecha de nacimiento. Debe ser una fecha anterior a hoy.
    /// </param>
    /// <param name="genero">
    /// Género del paciente. Solo se acepta 'M', 'F' u 'O'.
    /// </param>
    /// <param name="email">Email opcional. Máximo 100 caracteres.</param>
    /// <param name="telefono">Teléfono opcional. Máximo 20 caracteres.</param>
    /// <param name="direccion">Dirección opcional. Máximo 200 caracteres.</param>
    /// <param name="ciudad">Ciudad opcional. Máximo 50 caracteres.</param>
    /// <param name="provincia">Provincia opcional. Máximo 50 caracteres.</param>
    /// <param name="codigoPostal">Código postal opcional. Máximo 10 caracteres.</param>
    /// <returns>Una instancia válida de <see cref="Paciente"/> con estado activo.</returns>
    /// <exception cref="ArgumentNullException">
    /// Se lanza si el DNI es nulo.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Se lanza si nombre/apellido son nulos/vacíos o exceden longitud,
    /// si la fecha de nacimiento es futura, o si el género no es válido.
    /// </exception>
    public static Paciente Crear(
        Dni dni,
        string nombre,
        string apellido,
        DateTime fechaNacimiento,
        char genero,
        string? email = null,
        string? telefono = null,
        string? direccion = null,
        string? ciudad = null,
        string? provincia = null,
        string? codigoPostal = null)
    {
        // Validar el Value Object DNI
        if (dni is null)
            throw new ArgumentNullException(nameof(dni), "El DNI no puede ser nulo.");

        // Validar nombre
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException(
                "El nombre del paciente no puede ser nulo ni estar vacío.",
                nameof(nombre));

        if (nombre.Trim().Length > 100)
            throw new ArgumentException(
                $"El nombre no puede exceder 100 caracteres. " +
                $"Se recibieron {nombre.Trim().Length} caracteres.",
                nameof(nombre));

        // Validar apellido
        if (string.IsNullOrWhiteSpace(apellido))
            throw new ArgumentException(
                "El apellido del paciente no puede ser nulo ni estar vacío.",
                nameof(apellido));

        if (apellido.Trim().Length > 100)
            throw new ArgumentException(
                $"El apellido no puede exceder 100 caracteres. " +
                $"Se recibieron {apellido.Trim().Length} caracteres.",
                nameof(apellido));

        // Validar que la fecha de nacimiento sea anterior a hoy
        if (fechaNacimiento.Date >= DateTime.UtcNow.Date)
            throw new ArgumentException(
                $"La fecha de nacimiento '{fechaNacimiento:yyyy-MM-dd}' debe ser anterior a la fecha actual.",
                nameof(fechaNacimiento));

        // Validar género: solo se aceptan los tres caracteres permitidos
        char generoNormalizado = char.ToUpper(genero);
        if (generoNormalizado != 'M' && generoNormalizado != 'F' && generoNormalizado != 'O')
            throw new ArgumentException(
                $"El género '{genero}' no es válido. Solo se aceptan: 'M' (Masculino), 'F' (Femenino), 'O' (Otro).",
                nameof(genero));

        // Validar longitudes de campos opcionales de texto
        if (email is not null && email.Length > 100)
            throw new ArgumentException(
                $"El email no puede exceder 100 caracteres. " +
                $"Se recibieron {email.Length} caracteres.", nameof(email));

        if (telefono is not null && telefono.Length > 20)
            throw new ArgumentException(
                $"El teléfono no puede exceder 20 caracteres. " +
                $"Se recibieron {telefono.Length} caracteres.", nameof(telefono));

        if (direccion is not null && direccion.Length > 200)
            throw new ArgumentException(
                $"La dirección no puede exceder 200 caracteres. " +
                $"Se recibieron {direccion.Length} caracteres.", nameof(direccion));

        if (ciudad is not null && ciudad.Length > 50)
            throw new ArgumentException(
                $"La ciudad no puede exceder 50 caracteres. " +
                $"Se recibieron {ciudad.Length} caracteres.", nameof(ciudad));

        if (provincia is not null && provincia.Length > 50)
            throw new ArgumentException(
                $"La provincia no puede exceder 50 caracteres. " +
                $"Se recibieron {provincia.Length} caracteres.", nameof(provincia));

        if (codigoPostal is not null && codigoPostal.Length > 10)
            throw new ArgumentException(
                $"El código postal no puede exceder 10 caracteres. " +
                $"Se recibieron {codigoPostal.Length} caracteres.", nameof(codigoPostal));

        var ahora = DateTime.UtcNow;

        return new Paciente
        {
            DNI = dni,
            Nombre = nombre.Trim(),
            Apellido = apellido.Trim(),
            FechaNacimiento = fechaNacimiento.Date, // Solo guardar la fecha, sin hora
            Genero = generoNormalizado,
            Email = email,
            Telefono = telefono,
            Direccion = direccion,
            Ciudad = ciudad,
            Provincia = provincia,
            CodigoPostal = codigoPostal,
            Activo = true,
            FechaCreacion = ahora,
            FechaUltimaModificacion = ahora
        };
    }

    /// <summary>
    /// Marca al paciente como inactivo en el sistema (soft delete).
    /// Los pacientes inactivos no pueden recibir nuevos turnos.
    /// Actualiza automáticamente <see cref="Entity.FechaUltimaModificacion"/>.
    /// </summary>
    public void Desactivar()
    {
        Activo = false;
        ActualizarFechaModificacion();
    }
}
