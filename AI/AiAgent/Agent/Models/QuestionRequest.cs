using System.Collections.Generic;

namespace AiAgent.Models
{
    public class QuestionRequest
    {
        public string Question { get; set; } = "";
        public List<string>? RelatedFiles { get; set; }
        public bool IsDeepMode { get; set; }
    }
} 