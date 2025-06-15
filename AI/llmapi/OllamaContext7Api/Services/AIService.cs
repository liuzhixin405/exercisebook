using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using OllamaContext7Api.Models;

namespace OllamaContext7Api.Services
{
    public class AIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AIService> _logger;
        private readonly AIServiceOptions _options;
        private readonly IFileService _fileService;
        private readonly ConcurrentQueue<ChatMemoryEntry> _chatMemory = new ConcurrentQueue<ChatMemoryEntry>();
        private readonly ConcurrentDictionary<string, List<float>> _fileContentEmbeddings = new ConcurrentDictionary<string, List<float>>();
        private readonly ConcurrentDictionary<string, string> _fileContents = new ConcurrentDictionary<string, string>();
        private const int MAX_CHAT_MEMORY_SIZE = 5;
        private const string SYSTEM_PROMPT_AGENT = """
            You are an autonomous agent capable of executing file system operations.
            Your task is to interpret user commands related to file management and respond with a JSON object detailing the intended operation and its parameters.
            If the user's request is not a file operation, respond with "{\"operation\": \"chat\", \"reasoning\": \"The user's query is a general question and not a file operation.\"}"
            
            Supported operations:
            - "create": To create a new file or directory.
                - Path: The full path to the file or directory.
                - Content: (Optional) The content for the file. Not applicable for directories.
            - "read": To read the content of a file or list the contents of a directory.
                - Path: The full path to the file or directory.
            - "update": To update the content of an existing file.
                - Path: The full path to the file.
                - Content: The new content for the file.
            - "delete": To delete a file or directory.
                - Path: The full path to the file or directory.
            - "list": To list files and directories within a given path.
                - Path: The directory path to list.

            Your response MUST be a JSON object conforming to the following structure:
            {
              "operation": "create" | "read" | "update" | "delete" | "list" | "chat" | "unknown",
              "path": "path/to/file_or_directory",
              "content": "file_content_if_applicable",
              "reasoning": "Explanation if operation is unknown or chat"
            }

            Examples:
            User: "创建一个名为 my_document.txt 的文件，内容是 'Hello World'"
            Response: {"operation": "create", "path": "my_document.txt", "content": "Hello World"}

            User: "读取 services/AIService.cs 的内容"
            Response: {"operation": "read", "path": "services/AIService.cs"}

            User: "删除 temporary_data 文件夹"
            Response: {"operation": "delete", "path": "temporary_data"}

            User: "列出当前目录下的所有文件"
            Response: {"operation": "list", "path": "."}
            
            User: "创建一个名为 MyDotNetProject 的项目"
            Response: {"operation": "create", "path": "MyDotNetProject"}

            User: "什么是大语言模型？"
            Response: {"operation": "chat", "reasoning": "The user's query is a general question and not a file operation."}

            User: "我不知道该做什么"
            Response: {"operation": "unknown", "reasoning": "The user's intent is unclear."}
            """;

        public AIService(
            HttpClient httpClient,
            ILogger<AIService> logger,
            IOptions<AIServiceOptions> options,
            IFileService fileService)
        {
            _httpClient = httpClient;
            _logger = logger;
            _options = options.Value;
            _fileService = fileService;

            // 设置HTTP客户端超时（流式响应需要更长时间）
            _httpClient.Timeout = TimeSpan.FromMinutes(10);
        }

        public async IAsyncEnumerable<string> GetAnswerStreamAsync(string question,
            [EnumeratorCancellation] CancellationToken cancellationToken = default, IEnumerable<string> relatedFiles = null, bool isDeepMode = false)
        {
            _logger.LogInformation($"开始处理问题: {question}");

            // Step 1: Attempt to interpret the question as a file operation command using LLM
            _logger.LogInformation("尝试将问题解释为文件操作命令...");
            AgentCommand agentCommand = null;
            string llmInitialResponse = "";

            try
            {
                var systemPromptContent = SYSTEM_PROMPT_AGENT;
                var userPromptContent = $"用户命令: {question}";

                _logger.LogInformation($"发送给LLM的系统提示: {{systemPromptContent}}");
                _logger.LogInformation($"发送给LLM的用户命令: {{userPromptContent}}");

                // For LMStudio, we use the chat completions API, which accepts a list of messages.
                // For Ollama, we send a single prompt with the system and user content combined.
                if (_options.Provider == "LMStudio")
                {
                    await foreach (var chunk in GetLMStudioStreamAsyncInternal(userPromptContent, cancellationToken, systemPromptContent))
                    {
                        llmInitialResponse += chunk;
                    }
                }
                else // Ollama
                {
                    var combinedPrompt = $"{systemPromptContent}\n\n{userPromptContent}";
                    await foreach (var chunk in GetOllamaStreamAsyncInternal(combinedPrompt, cancellationToken))
                    {
                        llmInitialResponse += chunk;
                    }
                }

                _logger.LogInformation($"LLM 初始响应 (原始): {llmInitialResponse}");

                // Attempt to parse the LLM's response as an AgentCommand
                // We need to be careful with partial JSON responses from streaming.
                // For now, we assume the initial response contains a complete JSON.
                // In a real-world scenario, you might need a more robust JSON streaming parser.

                // Find the first and last curly brace to extract a potential JSON
                var firstBrace = llmInitialResponse.IndexOf('{');
                var lastBrace = llmInitialResponse.LastIndexOf('}');

                if (firstBrace != -1 && lastBrace != -1 && lastBrace > firstBrace)
                {
                    var jsonCandidate = llmInitialResponse.Substring(firstBrace, lastBrace - firstBrace + 1);
                    _logger.LogInformation($"尝试解析的JSON候选: {jsonCandidate}");
                    try
                    {
                        agentCommand = JsonSerializer.Deserialize<AgentCommand>(jsonCandidate, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        _logger.LogInformation($"成功解析为 AgentCommand: Operation={(agentCommand?.OperationType)}, Path={(agentCommand?.Path ?? "N/A")}, Content={(agentCommand?.Content ?? "N/A")}, Reasoning={(agentCommand?.Reasoning ?? "N/A")}");
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogWarning(ex, "解析LLM响应为AgentCommand失败，可能是非JSON格式或不完整JSON。回退到聊天模式。");
                        // Fallback to chat if JSON parsing fails
                        agentCommand = null;
                    }
                }
                else
                {
                    _logger.LogInformation("LLM响应不包含完整的JSON结构，回退到聊天模式。");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "在尝试解释为文件操作命令时发生错误。回退到聊天模式。");
                agentCommand = null; // Ensure fallback to chat
            }

            if (agentCommand != null && agentCommand.OperationType != "chat" && agentCommand.OperationType != "unknown")
            {
                _logger.LogInformation($"检测到文件操作命令: {agentCommand.OperationType} (Path: {agentCommand.Path ?? "N/A"})");
                string resultMessage = "";
                try
                {
                    switch (agentCommand.OperationType.ToLowerInvariant())
                    {
                        case "create":
                            if (!string.IsNullOrEmpty(agentCommand.Path))
                            {
                                if (agentCommand.Path.Contains(".")) // Heuristic for file vs directory
                                {
                                    await _fileService.SaveFileContentAsync(agentCommand.Path, agentCommand.Content ?? "");
                                    resultMessage = $"文件 '{agentCommand.Path}' 已成功创建。";
                                }
                                else
                                {
                                    await _fileService.CreateFolderAsync(agentCommand.Path);
                                    resultMessage = $"文件夹 '{agentCommand.Path}' 已成功创建。";
                                }
                            }
                            else
                            {
                                resultMessage = "创建文件/文件夹失败：未提供路径。";
                            }
                            break;
                        case "read":
                            if (!string.IsNullOrEmpty(agentCommand.Path))
                            {
                                // Check if it's a directory
                                var items = await _fileService.ListCachedFilesAsync(Path.GetDirectoryName(agentCommand.Path) ?? ".");
                                var isDirectory = items.Children?.Any(f => f.Type == "directory" && f.Name.Equals(Path.GetFileName(agentCommand.Path), StringComparison.OrdinalIgnoreCase)) ?? false;

                                if (isDirectory)
                                {
                                    // It's a directory, list its contents
                                    var dirItems = await _fileService.ListCachedFilesAsync(agentCommand.Path);
                                    if (dirItems.Children?.Any() ?? false)
                                    {
                                        var fileList = string.Join("\n", dirItems.Children.Select(i => i.Type == "directory" ? $"[DIR] {i.Name}" : $"[FILE] {i.Name}"));
                                        resultMessage = $"目录 '{agentCommand.Path}' 的内容:\n{fileList}";
                                    }
                                    else
                                    {
                                        resultMessage = $"目录 '{agentCommand.Path}' 为空。";
                                    }
                                }
                                else
                                {
                                    // Assume it's a file, read content
                                    var content = await _fileService.ReadFileContentAsync(agentCommand.Path);
                                    resultMessage = $"文件 '{agentCommand.Path}' 的内容:\n{content}";
                                }
                            }
                            else
                            {
                                resultMessage = "读取文件/目录失败：未提供路径。";
                            }
                            break;
                        case "update":
                            if (!string.IsNullOrEmpty(agentCommand.Path) && agentCommand.Content != null)
                            {
                                // Assuming update only applies to files for now
                                await _fileService.SaveFileContentAsync(agentCommand.Path, agentCommand.Content); // SaveFileContentAsync overwrites
                                resultMessage = $"文件 '{agentCommand.Path}' 已成功更新。";
                            }
                            else
                            {
                                resultMessage = "更新文件失败：未提供路径或内容。";
                            }
                            break;
                        case "delete":
                            if (!string.IsNullOrEmpty(agentCommand.Path))
                            {
                                await _fileService.DeleteFileAsync(agentCommand.Path);
                                resultMessage = $"文件/文件夹 '{agentCommand.Path}' 已成功删除。";
                            }
                            else
                            {
                                resultMessage = "删除文件/文件夹失败：未提供路径。";
                            }
                            break;
                        case "list":
                            if (!string.IsNullOrEmpty(agentCommand.Path))
                            {
                                var items = await _fileService.ListCachedFilesAsync(agentCommand.Path);
                                if (items.Children?.Any() ?? false)
                                {
                                    var fileList = string.Join("\n", items.Children.Select(i => i.Type == "directory" ? $"[DIR] {i.Name}" : $"[FILE] {i.Name}"));
                                    resultMessage = $"目录 '{agentCommand.Path}' 的内容:\n{fileList}";
                                }
                                else
                                {
                                    resultMessage = $"目录 '{agentCommand.Path}' 为空或不存在。";
                                }
                            }
                            else
                            {
                                // Default to listing current directory if no path provided
                                var items = await _fileService.ListCachedFilesAsync(".");
                                if (items.Children?.Any() ?? false)
                                {
                                    var fileList = string.Join("\n", items.Children.Select(i => i.Type == "directory" ? $"[DIR] {i.Name}" : $"[FILE] {i.Name}"));
                                    resultMessage = $"当前目录内容:\n{fileList}";
                                }
                                else
                                {
                                    resultMessage = "当前目录为空。";
                                }
                            }
                            break;
                        default:
                            resultMessage = $"未知文件操作类型: {agentCommand.OperationType}";
                            break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"执行文件操作 '{(agentCommand.OperationType)}' 失败。");
                    resultMessage = $"执行操作失败：{ex.Message}";
                }
                yield return resultMessage; // Yield the result of the file operation
                yield break; // Stop further processing as command is handled
            }
            else // Fallback to existing chat logic if no command or "chat" or "unknown"
            {
                _logger.LogInformation($"LLM响应未被识别为文件操作命令 (操作类型: {(agentCommand?.OperationType ?? "N/A")})，或LLM指示进行聊天，执行常规问答模式。");

                // 处理相关文件并生成嵌入
                if (relatedFiles != null && relatedFiles.Any())
                {
                    foreach (var fileName in relatedFiles)
                    {
                        if (!_fileContents.ContainsKey(fileName))
                        {
                            try
                            {
                                var content = await _fileService.ReadFileContentAsync(fileName);
                                _fileContents[fileName] = content;
                                if (_options.EnableMemory && !string.IsNullOrEmpty(_options.EmbeddingModelName))
                                {
                                    var fileEmbedding = await VectorOperations.GetEmbeddingAsync(
                                        _httpClient,
                                        content,
                                        _options.Ollama.Url.Replace("/api/generate", "/api/embeddings"),
                                        _options.EmbeddingModelName);
                                    _fileContentEmbeddings[fileName] = fileEmbedding;
                                    _logger.LogInformation($"文件 '{fileName}' 内容及其嵌入已缓存。");
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex, $"处理文件 '{fileName}' 失败，将不包含其内容。");
                            }
                        }
                    }
                }

                var currentQuestionEmbedding = new List<float>();
                if (_options.EnableMemory && !string.IsNullOrEmpty(_options.EmbeddingModelName))
                {
                    try
                    {
                        currentQuestionEmbedding = await VectorOperations.GetEmbeddingAsync(
                            _httpClient,
                            question,
                            _options.Ollama.Url.Replace("/api/generate", "/api/embeddings"), // Assuming embeddings endpoint is /api/embeddings
                            _options.EmbeddingModelName);
                        _logger.LogInformation($"问题嵌入生成成功，维度: {currentQuestionEmbedding.Count}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "生成问题嵌入失败，将不使用记忆功能。");
                        currentQuestionEmbedding = new List<float>(); // Clear embedding if failed
                    }
                }

                var retrievedContext = "";
                if (currentQuestionEmbedding.Any()) 
                {
                    var contextBuilder = new StringBuilder();

                    // Retrieve relevant chat memories
                    if (_chatMemory.Any())
                    {
                        var relevantMemories = _chatMemory
                            .OrderByDescending(entry => VectorOperations.CosineSimilarity(currentQuestionEmbedding, entry.QuestionEmbedding))
                            .Take(2) // Get top 2 relevant memories
                            .ToList();

                        if (relevantMemories.Any())
                        {
                            _logger.LogInformation($"找到 {relevantMemories.Count} 条相关历史记忆。");
                            foreach (var memory in relevantMemories)
                            {
                                contextBuilder.AppendLine($"历史问题: {memory.Question}");
                                contextBuilder.AppendLine($"历史回答: {memory.Answer}");
                            }
                        }
                    }

                    // Retrieve relevant file contents (if any)
                    if (_options.EnableMemory && !string.IsNullOrEmpty(_options.EmbeddingModelName))
                    {
                        if (relatedFiles != null && relatedFiles.Any())
                        {
                            // 如果提供了特定相关文件，则使用所有这些文件的内容
                            foreach (var fileName in relatedFiles)
                            {
                                if (_fileContents.TryGetValue(fileName, out var fileContent))
                                {
                                    _logger.LogInformation($"使用指定相关文件内容: {fileName}");
                                    contextBuilder.AppendLine($"相关文件: {fileName}");
                                    contextBuilder.AppendLine($"文件内容: {fileContent}");
                                }
                            }
                        }
                        else if (_fileContentEmbeddings.Any()) // 如果没有指定相关文件，则回退到对所有缓存文件进行相似度搜索
                        {
                            var relevantFileEntries = _fileContentEmbeddings
                                .Select(kvp => new { FileName = kvp.Key, Embedding = kvp.Value })
                                .OrderByDescending(entry => VectorOperations.CosineSimilarity(currentQuestionEmbedding, entry.Embedding))
                                .Take(1) // 在没有指定相关文件时，保留顶部1个相关文件
                                .ToList();

                            foreach (var fileEntry in relevantFileEntries)
                            {
                                if (_fileContents.TryGetValue(fileEntry.FileName, out var fileContent))
                                {
                                    _logger.LogInformation($"找到相关文件内容: {fileEntry.FileName}");
                                    contextBuilder.AppendLine($"相关文件: {fileEntry.FileName}");
                                    contextBuilder.AppendLine($"文件内容: {fileContent}");
                                }
                            }
                        }
                    }

                    retrievedContext = contextBuilder.ToString();
                    _logger.LogInformation($"Retrieved Context for RAG: {retrievedContext}");
                }

                var prompt = await BuildOptimizedPromptAsync(question, cancellationToken, retrievedContext);

                string fullAnswer = "";
                if (isDeepMode)
                {
                        _logger.LogInformation("深度模式启用：使用LMStudio模型");
                        await foreach (var chunk in GetLMStudioStreamAsyncInternal(prompt, cancellationToken))
                        {
                            fullAnswer += chunk;
                            yield return chunk;
                        }    
                }
                else
                {
                        _logger.LogInformation("非深度模式：使用Ollama模型");
                        await foreach (var chunk in GetOllamaStreamAsyncInternal(prompt, cancellationToken))
                        {
                            fullAnswer += chunk;
                            yield return chunk;
                        }
                }

                // Store in memory cache
                if (_options.EnableMemory && !string.IsNullOrEmpty(_options.EmbeddingModelName) && !string.IsNullOrEmpty(fullAnswer))
                {
                    try
                    {
                        var answerEmbedding = await VectorOperations.GetEmbeddingAsync(
                            _httpClient,
                            fullAnswer,
                            _options.Ollama.Url.Replace("/api/generate", "/api/embeddings"),
                            _options.EmbeddingModelName);

                        var newMemoryEntry = new ChatMemoryEntry
                        {
                            Question = question,
                            Answer = fullAnswer,
                            QuestionEmbedding = currentQuestionEmbedding,
                            AnswerEmbedding = answerEmbedding,
                            Timestamp = DateTime.UtcNow
                        };

                        _chatMemory.Enqueue(newMemoryEntry);
                        while (_chatMemory.Count > MAX_CHAT_MEMORY_SIZE)
                        {
                            _chatMemory.TryDequeue(out _); // Remove oldest entry if exceeding size
                        }
                        _logger.LogInformation($"新对话已添加到记忆，当前记忆大小: {_chatMemory.Count}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "生成回答嵌入或存储记忆失败。");
                    }
                }
            }
        }

        public async Task<bool> CheckHealthAsync()
        {
            try
            {
                if (_options.Provider == "Ollama")
                {
                    var request = new
                    {
                        model = _options.Ollama.Model,
                        prompt = "Hello",
                        stream = false,
                        options = new { num_predict = 5 }
                    };

                    var response = await _httpClient.PostAsJsonAsync(_options.Ollama.Url, request);
                    return response.IsSuccessStatusCode;
                }
                else
                {
                    var request = new
                    {
                        model = _options.LMStudio.Model,
                        messages = new[] { new { role = "user", content = "Hello" } },
                        stream = false
                    };

                    var response = await _httpClient.PostAsJsonAsync(_options.LMStudio.Url, request);
                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "健康检查失败");
                return false;
            }
        }

        private async Task<string> BuildOptimizedPromptAsync(string prompt, CancellationToken cancellationToken, string retrievedContext)
        {
            var sb = new StringBuilder();
            // 系统指令 - 定义角色和核心要求
            sb.AppendLine("系统指令：你是一个专业的编程助手，专注于.NET开发。");
            sb.AppendLine("你的回答必须遵循以下规则：");
            sb.AppendLine("1. 使用中文输出");
            sb.AppendLine("2. 涉及代码时必须提供完整可运行的C#示例");
            sb.AppendLine("3. 所有技术术语需符合微软官方文档规范");
            sb.AppendLine();

            sb.AppendLine($"当前问题：{prompt}");
            sb.AppendLine();
            sb.AppendLine("请提供准确、简洁的答案，包含以下内容：");
            sb.AppendLine("- 技术实现细节");
            sb.AppendLine("- 代码示例（如有）");
            sb.AppendLine("- 最佳实践建议");

            if (!string.IsNullOrEmpty(retrievedContext))
            {
                sb.AppendLine();
                sb.AppendLine("相关历史记忆：");
                sb.AppendLine(retrievedContext);
            }

            return sb.ToString();
        }

        private async IAsyncEnumerable<string> GetOllamaStreamAsyncInternal(string prompt,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var request = new
            {
                type="ollama",
                model = _options.Ollama.Model,
                prompt = prompt,
                stream = true,
                options = new
                {
                    temperature = 0.3,
                    top_p = 0.9,
                    num_ctx = 4096,
                    num_predict = 1000
                }
            };

            _logger.LogInformation($"发送Ollama请求，提示词长度: {prompt.Length}");

            // 使用单独的方法来处理可能抛出异常的操作
            var streamResult = await CreateStreamConnectionAsync(request, cancellationToken);

            if (!streamResult.Success)
            {
                yield return $"处理失败: {streamResult.ErrorMessage}";
                yield break;
            }

            using (var response = streamResult.HttpResponse)
            using (var stream = await response.Content.ReadAsStreamAsync())
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
                {
                    var line = await reader.ReadLineAsync();
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var parseSuccess = TryParseStreamResponse(line, out var streamResponse);

                    if (!parseSuccess)
                    {
                        _logger.LogWarning($"解析响应失败: {line}");
                        continue;
                    }

                    if (streamResponse?.Response != null)
                    {
                        yield return streamResponse.Response;
                    }

                    // 检查是否完成
                    if (streamResponse?.Done == true)
                    {
                        _logger.LogInformation("响应完成");
                        break;
                    }
                }
            }
        }

        private async Task<StreamConnectionResult> CreateStreamConnectionAsync(object request, CancellationToken cancellationToken)
        {
            try
            {
                var url = ((dynamic)request).type == "ollama"
                    ? _options.Ollama.Url 
                    : _options.LMStudio.Url;
                
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = JsonContent.Create(request)
                };

                var response = await _httpClient.SendAsync(httpRequest,
                    HttpCompletionOption.ResponseHeadersRead, cancellationToken);

                response.EnsureSuccessStatusCode();

                var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                var reader = new StreamReader(stream);

                return new StreamConnectionResult
                {
                    Success = true,
                    Reader = reader,
                    HttpResponse = response
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建连接失败");
                return new StreamConnectionResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        private bool TryParseStreamResponse(string line, out OllamaStreamResponse streamResponse)
        {
            streamResponse = null;
            try
            {
                // 处理SSE格式的响应 (可能以"data:"开头)
                var json = line.StartsWith("data:") 
                    ? line.Substring(5).Trim()
                    : line;

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                streamResponse = JsonSerializer.Deserialize<OllamaStreamResponse>(json, options);
                return true;
            }
            catch (JsonException ex)
            {
                _logger.LogWarning($"解析Ollama响应失败: {line}, 错误: {ex.Message}");
                return false;
            }
        }

        private bool TryParseLMStudioStreamResponse(string line, out LMStudioStreamResponse streamResponse)
        {
            streamResponse = null;
            try
            {
                // 处理SSE格式的响应 (可能以"data:"开头)
                var json = line.StartsWith("data:") 
                    ? line.Substring(5).Trim()
                    : line;

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                streamResponse = JsonSerializer.Deserialize<LMStudioStreamResponse>(json, options);
                return true;
            }
            catch (JsonException ex)
            {
                _logger.LogWarning($"解析LM Studio响应失败: {line}, 错误: {ex.Message}");
                return false;
            }
        }

        private async IAsyncEnumerable<string> GetLMStudioStreamAsyncInternal(string prompt,
            [EnumeratorCancellation] CancellationToken cancellationToken = default, string systemPrompt = null)
        {
            _logger.LogInformation($"向LMStudio发送请求... 模型: {_options.LMStudio.Model}");

            var messages = new List<object>();

            if (!string.IsNullOrEmpty(systemPrompt))
            {
                messages.Add(new { role = "system", content = systemPrompt });
            }

            messages.Add(new { role = "user", content = prompt });

            var request = new
            {
                type = "lmstudio",
                model = _options.LMStudio.Model,
                messages = messages,
                stream = true
            };

            var streamResult = await CreateStreamConnectionAsync(request, cancellationToken);
            using (var response = streamResult.HttpResponse)
            using (var stream = await response.Content.ReadAsStreamAsync())
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
                {
                    var line = await reader.ReadLineAsync();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    // LMStudio uses SSE format, data: {json}
                    if (line.StartsWith("data:"))
                    {
                        var json = line.Substring("data:".Length).Trim();
                        if (TryParseLMStudioStreamResponse(json, out var streamResponse))
                        {
                            yield return streamResponse.Choices[0].Delta.Content;
                        }
                    }
                }
            }
        }
    }
}
