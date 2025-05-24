using System.Net.Http.Json;
using System.Text;

namespace OllamaContext7Api.Services
{
    public class AIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly Context7Client _context7Client;
        private const string OllamaUrl = "http://localhost:11434/api/generate";
        private const string ModelName = "qwen2.5-coder:14b";

        public AIService(HttpClient httpClient, Context7Client context7Client)
        {
            _httpClient = httpClient;
            _context7Client = context7Client;
        }

        public async Task<string> GetAnswerAsync(string question)
        {
            return await ProcessWithContextAsync(question, "");
        }

        public async Task<string> ProcessWithContextAsync(string question, string context)
        {
            // 1. 构建Ollama提示词
            var prompt = BuildPrompt(question, context);
            
            // 2. 调用Ollama获取答案
            var ollamaResponse = await GetOllamaResponse(prompt);
            
            return ollamaResponse;
        }

        private async Task<string> GetContext7Docs(string topic)
        {
            // 1. 解析库ID
            var libraryId = await GetContext7LibraryId("netcore");
            
            // 2. 获取文档
            var docs = await GetLibraryDocs(libraryId, topic);
            
            return docs;
        }

        private async Task<string> GetContext7LibraryId(string libraryName)
        {
            return "/dotnet/aspnetcore.docs";
        }

        private async Task<string> GetLibraryDocs(string libraryId, string topic)
        {
            var docs = await GetContext7Docs(libraryId, topic);
            return docs ?? $"未能获取关于{topic}的文档";
        }

        private async Task<string> GetContext7Docs(string libraryId, string topic)
        {
            try
            {
                var docs = await GetMCPDocs(libraryId, topic);
                return docs;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取context7文档失败: {ex.Message}");
                return null;
            }
        }

        private async Task<string> GetMCPDocs(string libraryId, string topic)
        {
            // 使用MCP工具获取文档
            // 实际调用将由系统处理
            return "待获取的.NET Core文档内容";
        }

        private string BuildPrompt(string question, string context7Docs)
        {
            var sb = new StringBuilder();
            sb.AppendLine("基于以下.NET Core文档:");
            sb.AppendLine(context7Docs);
            sb.AppendLine($"请回答: {question}");
            return sb.ToString();
        }

        private async Task<string> GetOllamaResponse(string prompt)
        {
            var request = new
            {
                model = ModelName,
                prompt = prompt,
                stream = false
            };

            var response = await _httpClient.PostAsJsonAsync(OllamaUrl, request);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<OllamaResponse>();
            return result?.Response ?? "未能获取答案";
        }
    }

    public class OllamaResponse
    {
        public string Response { get; set; }
    }
}
