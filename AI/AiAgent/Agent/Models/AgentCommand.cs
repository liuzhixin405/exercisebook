using System.Text.Json.Serialization;

namespace AiAgent.Models
{
    public class AgentCommand
    {
        [JsonPropertyName("operation")]
        public string OperationType { get; set; } = "unknown";

        [JsonPropertyName("path")]
        public string? Path { get; set; }

        [JsonPropertyName("content")]
        public string? Content { get; set; }

        [JsonPropertyName("reasoning")]
        public string? Reasoning { get; set; }
    }
} 