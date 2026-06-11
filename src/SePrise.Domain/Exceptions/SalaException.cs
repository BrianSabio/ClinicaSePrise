namespace SePrise.Domain.Exceptions;

/// <summary>
/// Excepción lanzada cuando una operación sobre una sala o consultorio viola las reglas de negocio del dominio.
/// <br/>
/// <b>Usos típicos</b>:
/// <list type="bullet">
///   <item>Sala no encontrada: "No se encontró la sala con número '101'."</item>
///   <item>Sala inactiva: "La sala '101' está inactiva y no puede asignarse a nuevos turnos."</item>
///   <item>Sala ocupada: "La sala '101' ya tiene un turno asignado en el horario solicitado."</item>
///   <item>Tipo de sala incompatible: "La sala 'Espera' no es compatible con turnos de consulta."</item>
/// </list>
/// </summary>
public class SalaException : DomainException
{
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="SalaException"/> con un mensaje descriptivo.
    /// </summary>
    /// <param name="message">Mensaje que describe la regla de negocio de sala que fue violada.</param>
    public SalaException(string message) : base(message)
    {
    }

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="SalaException"/> con un mensaje descriptivo
    /// y una referencia a la excepción interna que causó este error.
    /// </summary>
    /// <param name="message">Mensaje que describe la regla de negocio de sala que fue violada.</param>
    /// <param name="innerException">
    /// Excepción de nivel inferior que originó este error. Puede ser <c>null</c>.
    /// </param>
    public SalaException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
