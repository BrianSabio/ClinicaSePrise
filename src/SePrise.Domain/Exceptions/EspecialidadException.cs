namespace SePrise.Domain.Exceptions;

/// <summary>
/// Excepción lanzada cuando una operación sobre una especialidad médica viola las reglas de negocio del dominio.
/// <br/>
/// <b>Usos típicos</b>:
/// <list type="bullet">
///   <item>Especialidad no encontrada: "No se encontró la especialidad con id '5'."</item>
///   <item>Especialidad inactiva: "La especialidad 'Cardiología' está inactiva y no puede recibir turnos."</item>
///   <item>Duración inválida: "La duración de consulta para 'Cardiología' es menor al mínimo permitido (15 min)."</item>
///   <item>Turno duplicado: "La especialidad 'Odontología' no permite múltiples turnos por día para el mismo paciente."</item>
/// </list>
/// </summary>
public class EspecialidadException : DomainException
{
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="EspecialidadException"/> con un mensaje descriptivo.
    /// </summary>
    /// <param name="message">Mensaje que describe la regla de negocio de especialidad que fue violada.</param>
    public EspecialidadException(string message) : base(message)
    {
    }

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="EspecialidadException"/> con un mensaje descriptivo
    /// y una referencia a la excepción interna que causó este error.
    /// </summary>
    /// <param name="message">Mensaje que describe la regla de negocio de especialidad que fue violada.</param>
    /// <param name="innerException">
    /// Excepción de nivel inferior que originó este error. Puede ser <c>null</c>.
    /// </param>
    public EspecialidadException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
