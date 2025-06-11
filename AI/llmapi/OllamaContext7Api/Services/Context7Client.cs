using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Json;

namespace OllamaContext7Api.Services
{
    public class Context7Client
    {
        private readonly string _serverName = "context7";
        private readonly HttpClient _httpClient;

        public Context7Client(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> ResolveLibraryId(string libraryName)
        {
            // 通过HTTP调用MCP服务器API
            var response = await _httpClient.PostAsJsonAsync(
                $"http://localhost:11434/mcp/{_serverName}/resolve-library-id",
                new { libraryName });
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            return "/dotnet/aspnetcore.docs";
        }

        public async Task<string> GetLibraryDocs(string libraryId, string topic, int tokens = 2000)
        {
            // 通过HTTP调用MCP服务器API
            var response = await _httpClient.PostAsJsonAsync(
                $"http://localhost:11434/mcp/{_serverName}/get-library-docs",
                new { 
                    context7CompatibleLibraryID = libraryId,
                    topic,
                    tokens
                });
            
            return response.IsSuccessStatusCode 
                ? await response.Content.ReadAsStringAsync()
                : $"未能获取关于{topic}的文档";
        }
    }
}
