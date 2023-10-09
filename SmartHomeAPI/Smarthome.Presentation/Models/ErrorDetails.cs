using System.Text.Json;

namespace Smarthome.Presentation.Models;

public class ErrorDetails
{
    public string? Message { get; set; }
    public string? ClassName { get; set; }
    public ErrorDetails? InnerException { get; set; }
    public List<string>? StackTrace { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
