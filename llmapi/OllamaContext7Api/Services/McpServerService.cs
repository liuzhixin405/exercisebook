using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace OllamaContext7Api.Services
{



    public class McpServerService
    {
        private Process _process;
        private StreamWriter _inputWriter;
        private StreamReader _outputReader;

        public void StartMcpServer()
        {
            if (_process != null && !_process.HasExited)
                return;

            var psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
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
        }


        public async Task<string> GetLibraryDocsAsync(string libraryName, string topic = null)
        {
            if (_process == null || _process.HasExited)
                throw new InvalidOperationException("MCP Server not running.");

            // 1. 解析库ID
            var resolveRequest = new {
                jsonrpc = "2.0",
                id = 1,
                method = "tools/call",
                @params = new {
                    name = "resolve-library-id",
                    arguments = new { libraryName = libraryName }
                }
            };

            await _inputWriter.WriteLineAsync(JsonSerializer.Serialize(resolveRequest));
            await _inputWriter.FlushAsync();

            var resolveResponse = await ReadWithTimeoutAsync(_outputReader, new CancellationTokenSource(5000).Token);
            var libraryId = ExtractLibraryId(resolveResponse);

            // 2. 获取文档
            var docsRequest = new {
                jsonrpc = "2.0",
                id = 2,
                method = "tools/call",
                @params = new {
                    name = "get-library-docs",
                    arguments = new { 
                        context7CompatibleLibraryID = libraryId,
                        topic = topic,
                        tokens = 10000 
                    }
                }
            };

            await _inputWriter.WriteLineAsync(JsonSerializer.Serialize(docsRequest));
            await _inputWriter.FlushAsync();

            var docsResponse = await ReadWithTimeoutAsync(_outputReader, new CancellationTokenSource(5000).Token);
            return docsResponse;
        }

        private string ExtractLibraryId(string jsonResponse)
        {
            try
            {
                using var doc = JsonDocument.Parse(jsonResponse);
                var root = doc.RootElement;
                
                // 检查错误响应
                if (root.TryGetProperty("error", out var error))
                {
                    Console.WriteLine($"MCP错误: {error}");
                    throw new InvalidOperationException($"MCP服务错误: {error}");
                }

                // 尝试从标准响应中提取库ID
                if (root.TryGetProperty("result", out var result))
                {
                    // 检查直接返回库ID的情况
                    if (result.ValueKind == JsonValueKind.String)
                        return result.GetString();

                    // 检查包含content数组的情况
                    if (result.TryGetProperty("content", out var content) && 
                        content.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in content.EnumerateArray())
                        {
                            // 检查文本内容中的库ID
                            if (item.TryGetProperty("text", out var text))
                            {
                                var textValue = text.GetString();
                                var patterns = new[] {
                                    @"Library ID:\s*([^\s]+)",
                                    @"Selected library:\s*([^\s]+)",
                                    @"ID:\s*([^\s]+)"
                                };

                                foreach (var pattern in patterns)
                                {
                                    var match = System.Text.RegularExpressions.Regex.Match(
                                        textValue, pattern);
                                    if (match.Success)
                                    {
                                        return match.Groups[1].Value;
                                    }
                                }
                            }

                            // 检查直接包含库ID的情况
                            if (item.TryGetProperty("libraryId", out var libId))
                                return libId.GetString();
                        }
                    }
                }

                throw new InvalidOperationException("无法从MCP响应中解析库ID");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"解析库ID失败: {ex.Message}");
                throw new InvalidOperationException("解析MCP响应时出错", ex);
            }
        }

        private async Task<string> ReadWithTimeoutAsync(StreamReader reader, CancellationToken token)
        {
            var sb = new StringBuilder();
            while (!token.IsCancellationRequested)
            {
                var line = await reader.ReadLineAsync();
                if (!string.IsNullOrWhiteSpace(line))
                {
                    sb.AppendLine(line);

                    // 🚩可加逻辑判断回答是否结束，如匹配 JSON 结尾、某标识符等
                    if (line.EndsWith("```") || line.EndsWith("}"))
                        break;
                }
                else
                {
                    await Task.Delay(100, token); // 避免 busy wait
                }
            }
            return sb.ToString().Trim();
        }

    }
}
