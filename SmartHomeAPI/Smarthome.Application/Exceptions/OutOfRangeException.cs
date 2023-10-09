namespace Smarthome.Application.Exceptions;

public class OutOfRangeException : Exception
{
    public OutOfRangeException(string message)
        : base(message) { }
}
