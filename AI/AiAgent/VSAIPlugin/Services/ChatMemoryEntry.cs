using System;

namespace VSAIPluginNew.Services
{
    public class ChatMemoryEntry
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public DateTime Timestamp { get; set; }
        // 可扩展：public float[] Embedding { get; set; }
    }
} 