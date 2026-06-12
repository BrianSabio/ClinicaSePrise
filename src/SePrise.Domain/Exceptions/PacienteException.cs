namespace SePrise.Domain.Exceptions;
public class PacienteException : DomainException
{
public PacienteException(string message) : base(message)
    {
    }
    public PacienteException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}


