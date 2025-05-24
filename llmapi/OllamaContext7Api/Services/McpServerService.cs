using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace OllamaContext7Api.Services
{
    public class McpServerService
    {
        private Process _process;
        private StreamWriter _inputWriter;
        private StreamReader _outputReader;
        private int _requestId = 1;
        private bool _initialized = false;

        public async Task StartMcpServerAsync()
        {
            if (_process != null && !_process.HasExited)
                return;

            var psi = new ProcessStartInfo
            {
                FileName = "cmd",
                Arguments = "/c npx -y @upstash/context7-mcp@latest",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            _process = new Process { StartInfo = psi };
            _process.Start();

            _inputWriter = _process.StandardInput;
            _outputReader = _process.StandardOutput;

            // 等待服务器启动
            await Task.Delay(3000);

            // 初始化MCP连接
            if (!_initialized)
            {
                await InitializeMcpAsync();
                _initialized = true;
            }
        }

        private async Task InitializeMcpAsync()
        {
            try
            {
                // 发送初始化请求
                var initRequest = new
                {
                    jsonrpc = "2.0",
                    id = _requestId++,
                    method = "initialize",
                    @params = new
                    {
                        protocolVersion = "2024-11-05",
                        capabilities = new { },
                        clientInfo = new
                        {
                            name = "OllamaContext7Api",
                            version = "1.0.0"
                        }
                    }
                };

                await SendRequestAsync(initRequest);
                var initResponse = await ReadResponseAsync();
                Console.WriteLine($"MCP初始化响应: {initResponse}");

                // 发送initialized通知
                var initializedNotification = new
                {
                    jsonrpc = "2.0",
                    method = "notifications/initialized"
                };

                await SendRequestAsync(initializedNotification);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MCP初始化失败: {ex.Message}");
                throw new InvalidOperationException("MCP服务器初始化失败", ex);
            }
        }

        public async Task<string> GetLibraryDocsAsync(string libraryName, string topic = null)
        {
            try
            {
                if (_process == null || _process.HasExited)
                {
                    await StartMcpServerAsync();
                }

                Console.WriteLine($"开始查询库: {libraryName}, 主题: {topic}");

                // 第一步：解析库ID
                var libraryId = await ResolveLibraryIdAsync(libraryName);
                if (string.IsNullOrEmpty(libraryId))
                {
                    throw new InvalidOperationException($"无法解析库 '{libraryName}' 的ID");
                }

                Console.WriteLine($"解析到库ID: {libraryId}");

                // 第二步：获取文档
                var docs = await GetDocsAsync(libraryId, topic);
                return docs;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取文档失败: {ex.Message}");
                throw new InvalidOperationException($"获取 {libraryName} 文档失败", ex);
            }
        }

        private async Task<string> ResolveLibraryIdAsync(string libraryName)
        {
            var request = new
            {
                jsonrpc = "2.0",
                id = _requestId++,
                method = "tools/call",
                @params = new
                {
                    name = "resolve-library-id",
                    arguments = new { libraryName = libraryName }
                }
            };

            await SendRequestAsync(request);
            var response = await ReadResponseAsync();

            Console.WriteLine($"Resolve响应: {response}");

            return ExtractLibraryIdFromResponse(response);
        }

        private async Task<string> GetDocsAsync(string libraryId, string topic)
        {
            var request = new
            {
                jsonrpc = "2.0",
                id = _requestId++,
                method = "tools/call",
                @params = new
                {
                    name = "get-library-docs",
                    arguments = new
                    {
                        context7CompatibleLibraryID = libraryId,
                        topic = topic ?? "",
                        tokens = 8000
                    }
                }
            };

            await SendRequestAsync(request);
            var response = await ReadResponseAsync();

            Console.WriteLine($"文档响应: {response}");

            return ExtractDocsFromResponse(response);
        }

        private string ExtractLibraryIdFromResponse(string jsonResponse)
        {
            try
            {
                using var doc = JsonDocument.Parse(jsonResponse);
                var root = doc.RootElement;

                // 检查错误
                if (root.TryGetProperty("error", out var error))
                {
                    Console.WriteLine($"Resolve错误: {error}");
                    return null;
                }

                // 检查结果
                if (root.TryGetProperty("result", out var result))
                {
                    // 检查content数组
                    if (result.TryGetProperty("content", out var content) &&
                        content.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in content.EnumerateArray())
                        {
                            if (item.TryGetProperty("type", out var type) &&
                                type.GetString() == "text" &&
                                item.TryGetProperty("text", out var text))
                            {
                                var textContent = text.GetString();
                                Console.WriteLine($"解析文本内容: {textContent}");

                                var libraryId = ExtractLibraryIdFromText(textContent);
                                if (!string.IsNullOrEmpty(libraryId))
                                {
                                    return libraryId;
                                }
                            }
                        }
                    }
                }

                Console.WriteLine("未能从响应中提取库ID");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"解析库ID响应失败: {ex.Message}");
                return null;
            }
        }

        private string ExtractLibraryIdFromText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            Console.WriteLine($"开始从文本中提取库ID: {text}");

            // 首先尝试预定义的库映射
            var libraryMappings = GetLibraryMappings();
            foreach (var mapping in libraryMappings)
            {
                if (text.ToLower().Contains(mapping.Key.ToLower()))
                {
                    Console.WriteLine($"通过映射找到库ID: {mapping.Value}");
                    return mapping.Value;
                }
            }

            // 改进的正则表达式模式
            var patterns = new[]
            {
                // 匹配 "Library ID: xxx" 或 "ID: xxx"
                @"(?:Library\s+)?ID\s*[:\s]\s*['""]?([a-zA-Z0-9_.-]+/[a-zA-Z0-9_.-]+)['""]?",
                
                // 匹配 "Selected library: xxx"
                @"Selected\s+library\s*[:\s]\s*['""]?([a-zA-Z0-9_.-]+/[a-zA-Z0-9_.-]+)['""]?",
                
                // 匹配 org/repo 格式
                @"\b([a-zA-Z0-9_-]+/[a-zA-Z0-9_.-]+)\b",
                
                // 匹配域名格式
                @"\b([a-zA-Z0-9_-]+\.[a-zA-Z0-9_.-]+)\b",
                
                // 匹配带引号的ID
                @"['""]([a-zA-Z0-9_.-]+/[a-zA-Z0-9_.-]+)['""]",
                
                // 匹配简单的库名
                @"\b([a-zA-Z0-9_-]{3,})\b"
            };

            foreach (var pattern in patterns)
            {
                var matches = Regex.Matches(text, pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                foreach (Match match in matches)
                {
                    if (match.Success && match.Groups.Count > 1)
                    {
                        var candidateId = match.Groups[1].Value.Trim();

                        // 验证候选ID
                        if (IsValidLibraryId(candidateId))
                        {
                            Console.WriteLine($"正则匹配到有效库ID: {candidateId}");
                            return candidateId;
                        }
                    }
                }
            }

            // 如果还是没有找到，尝试查找任何包含 / 的字符串
            var slashMatches = Regex.Matches(text, @"([a-zA-Z0-9_-]+/[a-zA-Z0-9_.-]+)");
            foreach (Match match in slashMatches)
            {
                var candidateId = match.Groups[1].Value.Trim();
                if (IsValidLibraryId(candidateId))
                {
                    Console.WriteLine($"斜杠模式匹配到库ID: {candidateId}");
                    return candidateId;
                }
            }

            Console.WriteLine($"无法从文本中提取库ID，文本内容: {text.Substring(0, Math.Min(500, text.Length))}");
            return null;
        }

        private Dictionary<string, string> GetLibraryMappings()
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                // .NET 相关
                ["dotnet"] = "dotnet/docs",
                [".net"] = "dotnet/docs",
                ["aspnet"] = "dotnet/AspNetCore.Docs",
                ["asp.net"] = "dotnet/AspNetCore.Docs",
                ["entityframework"] = "dotnet/EntityFramework.Docs",
                ["ef"] = "dotnet/EntityFramework.Docs",

                // JavaScript/Node.js
                ["react"] = "facebook/react",
                ["vue"] = "vuejs/vue",
                ["angular"] = "angular/angular",
                ["express"] = "expressjs/express",
                ["nodejs"] = "nodejs/node",
                ["node"] = "nodejs/node",

                // Python
                ["python"] = "python/cpython",
                ["django"] = "django/django",
                ["flask"] = "pallets/flask",
                ["fastapi"] = "tiangolo/fastapi",

                // Java
                ["spring"] = "spring-projects/spring-framework",
                ["springboot"] = "spring-projects/spring-boot",

                // 其他
                ["tensorflow"] = "tensorflow/tensorflow",
                ["pytorch"] = "pytorch/pytorch",
                ["kubernetes"] = "kubernetes/kubernetes",
                ["docker"] = "docker/docs"
            };
        }

        private bool IsValidLibraryId(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || id.Length < 3)
                return false;

            // 排除一些无效的常见字符串
            var invalidIds = new[] { "the", "and", "for", "with", "are", "this", "that", "from", "they", "have", "been", "said", "each", "which", "what", "where", "when", "how", "why", "can", "will", "would", "could", "should", "may", "might" };
            if (invalidIds.Contains(id.ToLower()))
                return false;

            // 验证格式：应该包含 / 或 . 或者是纯字母数字
            if (id.Contains('/'))
            {
                var parts = id.Split('/');
                return parts.Length == 2 &&
                       parts.All(p => !string.IsNullOrWhiteSpace(p) && p.Length > 0 &&
                                     Regex.IsMatch(p, @"^[a-zA-Z0-9_.-]+$"));
            }

            if (id.Contains('.'))
            {
                return Regex.IsMatch(id, @"^[a-zA-Z0-9_.-]+$") && id.Length >= 5;
            }

            // 纯字母数字的情况，长度至少要4个字符
            return Regex.IsMatch(id, @"^[a-zA-Z0-9_-]+$") && id.Length >= 4;
        }

        private string ExtractDocsFromResponse(string jsonResponse)
        {
            try
            {
                using var doc = JsonDocument.Parse(jsonResponse);
                var root = doc.RootElement;

                // 检查错误
                if (root.TryGetProperty("error", out var error))
                {
                    var errorMsg = error.GetProperty("message").GetString();
                    Console.WriteLine($"获取文档错误: {errorMsg}");
                    return $"获取文档失败: {errorMsg}";
                }

                // 提取文档内容
                if (root.TryGetProperty("result", out var result))
                {
                    if (result.TryGetProperty("content", out var content) &&
                        content.ValueKind == JsonValueKind.Array)
                    {
                        var docs = new StringBuilder();
                        foreach (var item in content.EnumerateArray())
                        {
                            if (item.TryGetProperty("type", out var type) &&
                                type.GetString() == "text" &&
                                item.TryGetProperty("text", out var text))
                            {
                                docs.AppendLine(text.GetString());
                            }
                        }

                        var docsContent = docs.ToString().Trim();
                        Console.WriteLine($"提取到文档内容长度: {docsContent.Length}");
                        return docsContent;
                    }
                }

                return "未找到文档内容";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"解析文档响应失败: {ex.Message}");
                return $"解析文档失败: {ex.Message}";
            }
        }

        private async Task SendRequestAsync(object request)
        {
            var json = JsonSerializer.Serialize(request);
            Console.WriteLine($"发送请求: {json}");

            await _inputWriter.WriteLineAsync(json);
            await _inputWriter.FlushAsync();
        }

        private async Task<string> ReadResponseAsync()
        {
            try
            {
                var response = await _outputReader.ReadLineAsync();
                if (string.IsNullOrEmpty(response))
                {
                    // 等待并重试
                    await Task.Delay(1000);
                    response = await _outputReader.ReadLineAsync();
                }

                Console.WriteLine($"收到响应: {response}");
                return response ?? "";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"读取响应失败: {ex.Message}");
                throw new InvalidOperationException("读取MCP响应失败", ex);
            }
        }

        public void Dispose()
        {
            try
            {
                _inputWriter?.Close();
                _outputReader?.Close();
                _process?.Kill();
                _process?.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"释放MCP服务资源时出错: {ex.Message}");
            }
        }
    }
}