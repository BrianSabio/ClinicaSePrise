namespace SePrise.Domain.Exceptions;

/// <summary>
/// Excepción lanzada cuando una operación sobre una atención médica viola las reglas de negocio del dominio.
/// <br/>
/// <b>Usos típicos</b>:
/// <list type="bullet">
///   <item>Inicio desde estado incorrecto: "No se puede iniciar la atención en estado 'Finalizada'."</item>
///   <item>Incoherencia temporal: "La hora de inicio no puede ser anterior a la hora de acreditación."</item>
///   <item>Finalización inválida: "Solo las atenciones en progreso pueden finalizarse."</item>
///   <item>Actualización en estado terminal: "No se pueden actualizar las notas de una atención en estado terminal."</item>
///   <item>Cancelación inválida: "No se puede cancelar la atención en estado 'Finalizada'."</item>
/// </list>
/// </summary>
public class AtencionException : DomainException
{
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="AtencionException"/> con un mensaje descriptivo.
    /// </summary>
    /// <param name="message">Mensaje que describe la regla de negocio de atención que fue violada.</param>
    public AtencionException(string message) : base(message)
    {
    }

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="AtencionException"/> con un mensaje descriptivo
    /// y una referencia a la excepción interna que causó este error.
    /// </summary>
    /// <param name="message">Mensaje que describe la regla de negocio de atención que fue violada.</param>
    /// <param name="innerException">
    /// Excepción de nivel inferior que originó este error. Puede ser <c>null</c>.
    /// </param>
    public AtencionException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
