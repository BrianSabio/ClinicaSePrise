namespace SePrise.Application.Exceptions;

/// <summary>
/// Excepción base para errores de la capa de aplicación (reglas de orquestación y casos de uso).
/// </summary>
public class ApplicationException : Exception
{
    public ApplicationException(string message) : base(message) { }
}
