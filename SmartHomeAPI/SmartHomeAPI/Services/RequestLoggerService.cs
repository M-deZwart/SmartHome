using Microsoft.AspNetCore.Http;
using SmartHomeAPI.Interfaces;

namespace SmartHomeAPI.Services
{
    public class RequestLoggerService : IRequestLogger
    {
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RequestLoggerService(ILogger logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public void LogRequest(string action, object result, DateTime timestamp)
        {
            var request = _httpContextAccessor?.HttpContext?.Request;
            var requestMethod = request?.Method ?? "Unknown Method";
            var requestPath = request?.Path ?? "Unknown Path";
            var requestBody = request?.Body.ToString() ?? "Unknown Body";

            var responseBody = result.ToString();

            _logger.LogInformation("Request: {requestMethod} {requestPath} {requestBody}");
            _logger.LogInformation("Response: {responseBody}");
            _logger.LogInformation("Timestamp: {timestamp}");
        }
    }
}
