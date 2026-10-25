namespace AiDevPlatform.Shared.DTOs.Deployment;

public class ConnectionTestResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? ErrorDetails { get; set; }
    public long ResponseTimeMs { get; set; }
    public DateTime TestedAt { get; set; } = DateTime.UtcNow;
    public string? TestedUrl { get; set; }
    public string? Environment { get; set; }
    public Dictionary<string, string> TestResults { get; set; } = new();
}
