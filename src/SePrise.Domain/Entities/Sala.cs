namespace SePrise.Domain.Entities;

/// <summary>
/// Tipos de salas o consultorios físicos disponibles en la clínica.
/// Clasifican el uso y equipamiento esperado de cada espacio.
/// </summary>
public enum TipoSala
{
    /// <summary>Sala destinada a consultas médicas generales o de especialidad.</summary>
    Consultorio = 1,

    /// <summary>Sala equipada para procedimientos médicos o técnicos especializados.</summary>
    Procedimientos = 2,

    /// <summary>Sala de espera para pacientes pendientes de atención.</summary>
    Espera = 3
}

/// <summary>
/// Entidad que representa una sala o consultorio físico dentro de la clínica.
/// Cada sala se identifica por un número único y un tipo que define su uso.
/// </summary>
public class Sala : Entity
{
    /// <summary>
    /// Identificador único de la sala (clave primaria, generado por la base de datos).
    /// </summary>
    public int IdSala { get; private set; }

    /// <summary>
    /// Número o código identificatorio de la sala (por ejemplo: "101", "Piso 2 - Sala A").
    /// Es único en la base de datos. Longitud máxima: 50 caracteres.
    /// </summary>
    public string Numero { get; private set; } = string.Empty;

    /// <summary>
    /// Tipo de sala que define su uso y equipamiento esperado.
    /// </summary>
    public TipoSala TipoSala { get; private set; }

    /// <summary>
    /// Indica si la sala está activa y disponible para asignar turnos.
    /// Las salas inactivas no se eliminan físicamente (soft delete).
    /// </summary>
    public bool Activo { get; private set; }

    /// <summary>
    /// Constructor privado sin parámetros requerido por Entity Framework Core
    /// para la materialización de entidades desde la base de datos.
    /// </summary>
    private Sala() { }

    /// <summary>
    /// Factory method para crear una nueva sala con los valores validados.
    /// Garantiza que la sala se construya en un estado consistente con las
    /// invariantes del dominio.
    /// </summary>
    /// <param name="numero">
    /// Número o código de la sala. No puede ser nulo ni vacío. Máximo 50 caracteres.
    /// </param>
    /// <param name="tipoSala">
    /// Tipo de sala según el enum <see cref="TipoSala"/>.
    /// Debe ser un valor válido definido en la enumeración.
    /// </param>
    /// <returns>Una instancia válida de <see cref="Sala"/> con estado activo.</returns>
    /// <exception cref="ArgumentException">
    /// Se lanza si el número de sala es nulo/vacío, excede los 50 caracteres,
    /// o si el tipo de sala no es un valor válido del enum.
    /// </exception>
    public static Sala Crear(string numero, TipoSala tipoSala)
    {
        // Validar que el número de sala no sea nulo ni vacío
        if (string.IsNullOrWhiteSpace(numero))
            throw new ArgumentException(
                "El número de sala no puede ser nulo ni estar vacío.",
                nameof(numero));

        // Validar longitud máxima del número de sala
        if (numero.Trim().Length > 50)
            throw new ArgumentException(
                $"El número de sala no puede exceder 50 caracteres. " +
                $"Se recibieron {numero.Trim().Length} caracteres.",
                nameof(numero));

        // Validar que el tipo de sala sea un valor definido en el enum
        if (!Enum.IsDefined(typeof(TipoSala), tipoSala))
            throw new ArgumentException(
                $"El tipo de sala '{tipoSala}' no es un valor válido.",
                nameof(tipoSala));

        return new Sala
        {
            Numero = numero.Trim(),
            TipoSala = tipoSala,
            Activo = true,
            FechaCreacion = DateTime.UtcNow,
            FechaUltimaModificacion = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Marca la sala como inactiva en el sistema (soft delete).
    /// Las salas inactivas no aparecerán disponibles para nuevos turnos.
    /// Actualiza automáticamente <see cref="Entity.FechaUltimaModificacion"/>.
    /// </summary>
    public void Desactivar()
    {
        Activo = false;
        ActualizarFechaModificacion();
    }
}
