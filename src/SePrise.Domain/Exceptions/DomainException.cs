namespace SePrise.Domain.Exceptions;

/// <summary>
/// Clase base abstracta para todas las excepciones de dominio del sistema SePrise.
/// Representa un error originado en las reglas de negocio del dominio,
/// distinguiéndose de errores técnicos (IOException, ArgumentException, etc.).
/// <br/>
/// <b>Jerarquía de uso</b>:
/// <list type="bullet">
///   <item>Capturar <see cref="TurnoException"/> para errores específicos de turno.</item>
///   <item>Capturar <see cref="DomainException"/> para cualquier regla de negocio violada.</item>
///   <item>Nunca instanciar directamente: usar las subclases específicas.</item>
/// </list>
/// </summary>
public abstract class DomainException : Exception
{
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="DomainException"/> con un mensaje descriptivo.
    /// </summary>
    /// <param name="message">Mensaje que describe la regla de negocio violada.</param>
    protected DomainException(string message) : base(message)
    {
    }

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="DomainException"/> con un mensaje descriptivo
    /// y una referencia a la excepción interna que causó este error.
    /// </summary>
    /// <param name="message">Mensaje que describe la regla de negocio violada.</param>
    /// <param name="innerException">
    /// Excepción de nivel inferior que originó este error de dominio. Puede ser <c>null</c>.
    /// </param>
    protected DomainException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
