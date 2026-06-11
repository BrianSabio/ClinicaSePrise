namespace SePrise.Domain.Exceptions;

/// <summary>
/// Excepción lanzada cuando una operación sobre un paciente viola las reglas de negocio del dominio.
/// <br/>
/// <b>Usos típicos</b>:
/// <list type="bullet">
///   <item>Paciente no encontrado: "No se encontró un paciente con DNI '12345678'."</item>
///   <item>DNI duplicado: "Ya existe un paciente registrado con el DNI '12345678'."</item>
///   <item>Paciente inactivo: "El paciente con id '5' está inactivo y no puede recibir nuevos turnos."</item>
///   <item>Operación inválida sobre paciente: "No se puede modificar un paciente con turnos activos."</item>
/// </list>
/// </summary>
public class PacienteException : DomainException
{
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="PacienteException"/> con un mensaje descriptivo.
    /// </summary>
    /// <param name="message">Mensaje que describe la regla de negocio de paciente que fue violada.</param>
    public PacienteException(string message) : base(message)
    {
    }

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="PacienteException"/> con un mensaje descriptivo
    /// y una referencia a la excepción interna que causó este error.
    /// </summary>
    /// <param name="message">Mensaje que describe la regla de negocio de paciente que fue violada.</param>
    /// <param name="innerException">
    /// Excepción de nivel inferior que originó este error. Puede ser <c>null</c>.
    /// </param>
    public PacienteException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
