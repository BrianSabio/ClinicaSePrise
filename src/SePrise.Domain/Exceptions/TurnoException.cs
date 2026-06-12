namespace SePrise.Domain.Exceptions;
public class TurnoException : DomainException
{
public TurnoException(string message) : base(message)
    {
    }
    public TurnoException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}


