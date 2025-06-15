using System;

namespace OllamaContext7Api.Models
{
    public class QuestionResponse
    {
        public string Question { get; set; } = "";
        public string Answer { get; set; } = "";
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
} 