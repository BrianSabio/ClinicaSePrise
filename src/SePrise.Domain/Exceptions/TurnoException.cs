namespace SePrise.Domain.Exceptions;

/// <summary>
/// Excepción lanzada cuando una operación sobre un turno viola las reglas de negocio del dominio.
/// <br/>
/// <b>Usos típicos</b>:
/// <list type="bullet">
///   <item>Transición de estado no permitida: "No se puede confirmar turno en estado 'Atendido'."</item>
///   <item>Cancelación con atención en progreso: "No se puede cancelar turno con atención en progreso."</item>
///   <item>Reprogramación inválida: "No se puede reprogramar turno en estado 'Cancelado'."</item>
///   <item>NoAsistio desde estado incorrecto: "Solo turnos Reservados pueden marcarse como NoAsistio."</item>
/// </list>
/// </summary>
public class TurnoException : DomainException
{
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="TurnoException"/> con un mensaje descriptivo.
    /// </summary>
    /// <param name="message">Mensaje que describe la regla de negocio de turno que fue violada.</param>
    public TurnoException(string message) : base(message)
    {
    }

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="TurnoException"/> con un mensaje descriptivo
    /// y una referencia a la excepción interna que causó este error.
    /// </summary>
    /// <param name="message">Mensaje que describe la regla de negocio de turno que fue violada.</param>
    /// <param name="innerException">
    /// Excepción de nivel inferior que originó este error. Puede ser <c>null</c>.
    /// </param>
    public TurnoException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
