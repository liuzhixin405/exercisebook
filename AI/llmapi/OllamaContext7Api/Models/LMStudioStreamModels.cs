using System.Collections.Generic;

namespace OllamaContext7Api.Models
{
    public class LMStudioStreamResponse
    {
        public string Id { get; set; } = "";
        public string Object { get; set; } = "";
        public long Created { get; set; }
        public string Model { get; set; } = "";
        public List<LMStudioChoice> Choices { get; set; } = new();
    }

    public class LMStudioChoice
    {
        public int Index { get; set; }
        public LMStudioDelta Delta { get; set; } = new();
        public object? FinishReason { get; set; }
    }

    public class LMStudioDelta
    {
        public string Content { get; set; } = "";
    }
} 