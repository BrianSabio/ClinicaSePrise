namespace SePrise.Domain.Exceptions;
public class AtencionException : DomainException
{
public AtencionException(string message) : base(message)
    {
    }
    public AtencionException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}


