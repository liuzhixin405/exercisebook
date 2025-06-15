using System;
using System.Collections.Generic;

namespace OllamaContext7Api.Models
{
    public class ChatMemoryEntry
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public List<float> QuestionEmbedding { get; set; }
        public List<float> AnswerEmbedding { get; set; }
        public DateTime Timestamp { get; set; }
    }
} 