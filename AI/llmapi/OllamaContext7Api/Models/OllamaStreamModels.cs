using System;
using System.Collections.Generic;

namespace OllamaContext7Api.Models
{
    public class OllamaStreamResponse
    {
        public string Response { get; set; } = "";
        public bool Done { get; set; }
        public string Model { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }

    public class OllamaEmbeddingResponse
    {
        public List<float> Embedding { get; set; } = new List<float>();
    }
} 