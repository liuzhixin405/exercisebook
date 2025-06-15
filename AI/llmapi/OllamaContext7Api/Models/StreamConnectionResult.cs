using System.IO;
using System.Net.Http;

namespace OllamaContext7Api.Models
{
    public class StreamConnectionResult
    {
        public bool Success { get; set; }
        public StreamReader? Reader { get; set; }
        public HttpResponseMessage? HttpResponse { get; set; } // Added this property
        public string? ErrorMessage { get; set; }
    }
}
