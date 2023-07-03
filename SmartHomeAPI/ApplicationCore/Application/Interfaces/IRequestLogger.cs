namespace Application.Application.Interfaces
{
    public interface IRequestLogger
    {
        void LogRequest(string action, object result, DateTime timestamp);
    }
}
