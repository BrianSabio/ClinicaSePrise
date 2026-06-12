namespace SePrise.Domain.Exceptions;
public class SalaException : DomainException
{
public SalaException(string message) : base(message)
    {
    }
    public SalaException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}


