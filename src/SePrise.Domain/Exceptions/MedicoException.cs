namespace SePrise.Domain.Exceptions;

/// <summary>
/// Excepción lanzada cuando una operación sobre un médico viola las reglas de negocio del dominio.
/// <br/>
/// <b>Usos típicos</b>:
/// <list type="bullet">
///   <item>Médico no encontrado: "No se encontró un médico con matrícula 'MP123456'."</item>
///   <item>Médico inactivo: "El médico con id '3' está inactivo y no puede recibir nuevos turnos."</item>
///   <item>Sin especialidades: "El médico con id '3' no tiene especialidades asignadas."</item>
///   <item>Especialidad no habilitada: "El médico no tiene habilitada la especialidad 'Cardiología'."</item>
/// </list>
/// </summary>
public class MedicoException : DomainException
{
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="MedicoException"/> con un mensaje descriptivo.
    /// </summary>
    /// <param name="message">Mensaje que describe la regla de negocio de médico que fue violada.</param>
    public MedicoException(string message) : base(message)
    {
    }

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="MedicoException"/> con un mensaje descriptivo
    /// y una referencia a la excepción interna que causó este error.
    /// </summary>
    /// <param name="message">Mensaje que describe la regla de negocio de médico que fue violada.</param>
    /// <param name="innerException">
    /// Excepción de nivel inferior que originó este error. Puede ser <c>null</c>.
    /// </param>
    public MedicoException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
