namespace SePrise.Domain.Exceptions;
public class MedicoException : DomainException
{
public MedicoException(string message) : base(message)
    {
    }
    public MedicoException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}


