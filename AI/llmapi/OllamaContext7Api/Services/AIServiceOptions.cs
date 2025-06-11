namespace OllamaContext7Api.Services
{
    public class AIServiceOptions
    {
        public const string SectionName = "AIService";
        
        public string Provider { get; set; } = "Ollama";
        
        public OllamaOptions Ollama { get; set; } = new();
        public LMStudioOptions LMStudio { get; set; } = new();
    }

    public class OllamaOptions
    {
        public string Url { get; set; } = "http://localhost:11434/api/generate";
        public string Model { get; set; } = string.Empty;
    }

    public class LMStudioOptions
    {
        public string Url { get; set; } = "http://localhost:1234/v1/chat/completions";
        public string Model { get; set; } = string.Empty;
    }
}
