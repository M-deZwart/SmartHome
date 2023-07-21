using Application.Application.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Infrastructure.Logging
{
    public class RequestLogger : IRequestLogger
    {
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RequestLogger(ILogger logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public void LogRequest(string action, object result, DateTime timestamp)
        {
            var request = _httpContextAccessor?.HttpContext?.Request;
            var requestMethod = request?.Method ?? "Unknown Method";
            var requestPath = request?.Path ?? "Unknown Path";
            var requestBody = request?.Body.ToString() ?? "Unknown request Body";

            var responseBody = result?.ToString() ?? "Unknown response Body";

            _logger.LogInformation("Request: {requestMethod} {requestPath} {requestBody}", requestMethod, requestPath, requestBody);
            _logger.LogInformation("Response: {responseBody}", responseBody);
            _logger.LogInformation("Timestamp: {timestamp}", timestamp);
        }
    }
}
