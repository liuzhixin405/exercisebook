using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VSAIPluginNew.AI
{
    public class OllamaAgent
    {
        private readonly HttpClient _httpClient;
        private readonly string _modelName;
        private readonly string _systemMessage;

        public OllamaAgent(string modelName, string systemMessage = "You are a helpful AI assistant")
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:11434")
            };
            _modelName = modelName;
            _systemMessage = systemMessage;
        }

        public async Task<string> GenerateReplyAsync(string promptText)
        {
            try
            {
                var messages = new List<Message>
                {
                    new Message { Role = "system", Content = _systemMessage },
                    new Message { Role = "user", Content = promptText }
                };

                var requestObject = new
                {
                    model = _modelName,
                    messages,
                    stream = false
                };

                var content = new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(requestObject),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync("/api/chat", content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var responseObject = System.Text.Json.JsonSerializer.Deserialize<OllamaResponse>(responseContent);

                return responseObject?.Message?.Content ?? "No response from model";
            }
            catch (Exception ex)
            {
                return $"Error generating reply: {ex.Message}";
            }
        }

        public async Task<string> ProcessFilesAndGenerateReplyAsync(string promptText, Dictionary<string, string> filesContent)
        {
            var contextBuilder = new StringBuilder();
            contextBuilder.AppendLine("I have access to the following files from your Visual Studio solution:");
            
            foreach (var file in filesContent)
            {
                contextBuilder.AppendLine($"File: {file.Key}");
                contextBuilder.AppendLine("```");
                contextBuilder.AppendLine(file.Value);
                contextBuilder.AppendLine("```");
            }

            contextBuilder.AppendLine("\n");
            contextBuilder.AppendLine(promptText);

            return await GenerateReplyAsync(contextBuilder.ToString());
        }
    }

    public class Message
    {
        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }
    }

    public class OllamaResponse
    {
        [JsonPropertyName("message")]
        public Message Message { get; set; }
    }
} 