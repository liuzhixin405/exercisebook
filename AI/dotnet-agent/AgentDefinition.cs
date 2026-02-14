using System.Text.Json.Serialization;

namespace DotnetAgent
{
    public enum AgentKind { Local, Remote }

    public class InputConfig
    {
        [JsonPropertyName("inputSchema")]
        public object? InputSchema { get; set; }
    }

    public class RunConfig
    {
        [JsonPropertyName("maxTurns")]
        public int? MaxTurns { get; set; }

        [JsonPropertyName("maxTimeMinutes")]
        public int? MaxTimeMinutes { get; set; }
    }

    public class AgentDefinition
    {
        [JsonPropertyName("kind")]
        public AgentKind Kind { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("systemPrompt")]
        public string? SystemPrompt { get; set; }

        [JsonPropertyName("model")]
        public string? Model { get; set; }

        [JsonPropertyName("inputConfig")]
        public InputConfig? InputConfig { get; set; }

        [JsonPropertyName("runConfig")]
        public RunConfig? RunConfig { get; set; }
    }
}
