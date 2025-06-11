using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OllamaAIAssistant
{
    // Ollama API 响应模型 (与之前相同)
    public class OllamaResponse
    {
        public string model { get; set; }
        public string response { get; set; }
        public bool done { get; set; }
        public string created_at { get; set; }
        public string content { get; set; } // For chat API response
    }

    // Ollama API 请求模型 (更新以支持消息列表)
    public class OllamaRequest
    {
        public string model { get; set; }
        public List<Message> messages { get; set; } // For chat API
        public string prompt { get; set; } // For generate API
        public bool stream { get; set; } = false;
        public Dictionary<string, object> options { get; set; }
    }

    // 消息模型 (用于构建对话历史)
    public class Message
    {
        public string role { get; set; } // "user", "assistant", "system"
        public string content { get; set; }
    }

    // Ollama 模型信息 (与之前相同)
    public class OllamaModel
    {
        public string name { get; set; }
        public string modified_at { get; set; }
        public long size { get; set; }
        public string digest { get; set; }
    }

    public class OllamaModelsResponse
    {
        public List<OllamaModel> models { get; set; }
    }

    public class OllamaAIService : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private List<Message> _conversationHistory; // 存储对话历史

        public OllamaAIService(string baseUrl = "http://localhost:11434")
        {
            _httpClient = new HttpClient();
            _baseUrl = baseUrl;
            _conversationHistory = new List<Message>();
        }

        // 清除当前对话历史
        public void ClearConversationHistory()
        {
            _conversationHistory.Clear();
        }

        // 获取可用模型列表 (与之前相同)
        public async Task<List<OllamaModel>> GetAvailableModelsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/tags");
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                var modelsResponse = JsonSerializer.Deserialize<OllamaModelsResponse>(jsonString);

                return modelsResponse?.models ?? new List<OllamaModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取模型列表失败: {ex.Message}");
                return new List<OllamaModel>();
            }
        }

        // 发送消息到指定模型 (现在支持对话历史)
        public async Task<string> SendMessageAsync(string model, string prompt, Dictionary<string, object> options = null)
        {
            // 添加用户消息到历史
            _conversationHistory.Add(new Message { role = "user", content = prompt });

            try
            {
                var request = new OllamaRequest
                {
                    model = model,
                    messages = _conversationHistory, // 发送整个对话历史
                    stream = false,
                    options = options ?? new Dictionary<string, object>()
                };

                var jsonContent = JsonSerializer.Serialize(request);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // 尝试使用 /api/chat 接口
                HttpResponseMessage response;
                try
                {
                    response = await _httpClient.PostAsync($"{_baseUrl}/api/chat", content);
                    response.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException chatEx)
                {
                    // 如果 /api/chat 不可用，回退到 /api/generate
                    Console.WriteLine($"Warn: /api/chat endpoint failed ({chatEx.Message}). Falling back to /api/generate.");
                    request.messages = null; // Clear messages for generate endpoint
                    request.prompt = string.Join("\n", _conversationHistory.Select(m => $"{m.role}: {m.content}")); // Flatten history for prompt
                    jsonContent = JsonSerializer.Serialize(request);
                    content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    response = await _httpClient.PostAsync($"{_baseUrl}/api/generate", content);
                    response.EnsureSuccessStatusCode();
                }

                var responseString = await response.Content.ReadAsStringAsync();
                var ollamaResponse = JsonSerializer.Deserialize<OllamaResponse>(responseString);

                string aiResponseContent = ollamaResponse?.response ?? ollamaResponse?.content ?? "无响应";

                // 添加 AI 响应到历史
                _conversationHistory.Add(new Message { role = "assistant", content = aiResponseContent });

                return aiResponseContent;
            }
            catch (Exception ex)
            {
                // 如果发生错误，移除最后一条用户消息，避免历史不一致
                if (_conversationHistory.Count > 0 && _conversationHistory[_conversationHistory.Count - 1].role == "user")
                {
                    _conversationHistory.RemoveAt(_conversationHistory.Count - 1);
                }
                return $"错误: {ex.Message}";
            }
        }

        // 检查 Ollama 服务是否可用 (与之前相同)
        public async Task<bool> IsServiceAvailableAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/tags");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8; // 确保中文输出正确

            Console.WriteLine("=== Ollama .NET AI 助手 ===");
            Console.WriteLine();

            using var aiService = new OllamaAIService();

            // 检查 Ollama 服务是否可用
            Console.WriteLine("检查 Ollama 服务连接...");
            if (!await aiService.IsServiceAvailableAsync())
            {
                Console.WriteLine("❌ 无法连接到 Ollama 服务");
                Console.WriteLine("请确保 Ollama 已安装并运行在 http://localhost:11434");
                Console.WriteLine("启动 Ollama: ollama serve");
                return;
            }
            Console.WriteLine("✅ Ollama 服务连接成功");
            Console.WriteLine();

            // 获取可用模型
            Console.WriteLine("获取可用模型...");
            var models = await aiService.GetAvailableModelsAsync();

            if (models.Count == 0)
            {
                Console.WriteLine("❌ 没有找到可用的模型");
                Console.WriteLine("请先下载模型，例如: ollama pull llama2");
                return;
            }

            Console.WriteLine("可用模型:");
            for (int i = 0; i < models.Count; i++)
            {
                var sizeInMB = models[i].size / (1024.0 * 1024.0); // 使用浮点数计算
                Console.WriteLine($"{i + 1}. {models[i].name} ({sizeInMB:F2} MB)"); // 格式化为两位小数
            }
            Console.WriteLine();

            // 选择模型
            string selectedModel = "";
            if (models.Count == 1)
            {
                selectedModel = models[0].name;
                Console.WriteLine($"使用模型: {selectedModel}");
            }
            else
            {
                Console.Write("请选择模型 (输入数字): ");
                if (int.TryParse(Console.ReadLine(), out int choice) &&
                    choice >= 1 && choice <= models.Count)
                {
                    selectedModel = models[choice - 1].name;
                    Console.WriteLine($"已选择模型: {selectedModel}");
                }
                else
                {
                    selectedModel = models[0].name;
                    Console.WriteLine($"使用默认模型: {selectedModel}");
                }
            }
            Console.WriteLine();

            // 设置模型参数
            var modelOptions = new Dictionary<string, object>
            {
                { "temperature", 0.7 },
                { "top_p", 0.9 },
                { "max_tokens", 2000 }
            };

            Console.WriteLine("AI 助手已准备就绪！输入 'quit' 退出，输入 'models' 查看模型列表");
            Console.WriteLine("输入 'switch' 切换模型，输入 'clear' 清空当前对话");
            Console.WriteLine(new string('-', 50));

            // 主对话循环
            while (true)
            {
                Console.Write("你: ");
                var userInput = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(userInput))
                    continue;

                if (userInput.ToLower() == "quit")
                {
                    Console.WriteLine("再见！");
                    break;
                }
                else if (userInput.ToLower() == "models")
                {
                    Console.WriteLine("\n可用模型:");
                    var currentModels = await aiService.GetAvailableModelsAsync();
                    for (int i = 0; i < currentModels.Count; i++)
                    {
                        var status = currentModels[i].name == selectedModel ? " [当前]" : "";
                        var sizeInMB = currentModels[i].size / (1024.0 * 1024.0);
                        Console.WriteLine($"{i + 1}. {currentModels[i].name} ({sizeInMB:F2} MB){status}");
                    }
                    Console.WriteLine();
                    continue;
                }
                else if (userInput.ToLower() == "switch")
                {
                    Console.WriteLine("\n选择新模型:");
                    var currentModels = await aiService.GetAvailableModelsAsync();
                    for (int i = 0; i < currentModels.Count; i++)
                    {
                        var sizeInMB = currentModels[i].size / (1024.0 * 1024.0);
                        Console.WriteLine($"{i + 1}. {currentModels[i].name} ({sizeInMB:F2} MB)");
                    }
                    Console.Write("请选择模型 (输入数字): ");
                    if (int.TryParse(Console.ReadLine(), out int newChoice) &&
                        newChoice >= 1 && newChoice <= currentModels.Count)
                    {
                        selectedModel = currentModels[newChoice - 1].name;
                        aiService.ClearConversationHistory(); // 切换模型时清空对话历史
                        Console.WriteLine($"已切换到模型: {selectedModel}，对话历史已清空。\n");
                    }
                    else
                    {
                        Console.WriteLine("无效选择，保持当前模型\n");
                    }
                    continue;
                }
                else if (userInput.ToLower() == "clear")
                {
                    aiService.ClearConversationHistory();
                    Console.WriteLine("对话历史已清空。\n");
                    continue;
                }

                Console.Write("AI: ");

                // 发送消息并获取响应
                var response = await aiService.SendMessageAsync(selectedModel, userInput, modelOptions);
                Console.WriteLine(response);
                Console.WriteLine();
            }
        }
    }
}