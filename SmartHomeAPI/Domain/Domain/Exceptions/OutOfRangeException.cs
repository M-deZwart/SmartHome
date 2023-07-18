namespace Domain.Domain.Exceptions;

public class OutOfRangeException : Exception
{
    public OutOfRangeException(string message) : base(message)
    {
    }
}
