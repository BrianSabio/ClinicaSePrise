namespace SePrise.Domain.Exceptions;
public class EspecialidadException : DomainException
{
public EspecialidadException(string message) : base(message)
    {
    }
    public EspecialidadException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}


