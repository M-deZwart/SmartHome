namespace Application.Application.Contracts
{
    public interface IRequestLogger
    {
        void LogRequest(string action, object result, DateTime timestamp);
    }
}
