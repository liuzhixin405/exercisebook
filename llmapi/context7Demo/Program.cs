using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace McpClient
{
    public class Context7Client
    {
        private Process mcpProcess;
        private StreamWriter stdin;
        private StreamReader stdout;
        private int requestId = 1;

        public async Task<bool> ConnectAsync()
        {
            try
            {
                // 启动Context7 MCP服务器进程
                var startInfo = new ProcessStartInfo
                {
                    FileName = "cmd",
                    Arguments = "/c npx -y @upstash/context7-mcp@latest",
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                mcpProcess = Process.Start(startInfo);
                if (mcpProcess == null)
                {
                    Console.WriteLine("无法启动MCP服务器进程");
                    return false;
                }

                stdin = mcpProcess.StandardInput;
                stdout = mcpProcess.StandardOutput;

                // 等待服务器启动
                await Task.Delay(2000);

                // 发送初始化请求
                await InitializeAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"连接失败: {ex.Message}");
                return false;
            }
        }

        private async Task InitializeAsync()
        {
            var initRequest = new
            {
                jsonrpc = "2.0",
                id = requestId++,
                method = "initialize",
                @params = new
                {
                    protocolVersion = "2024-11-05",
                    capabilities = new
                    {
                        roots = new { }
                    },
                    clientInfo = new
                    {
                        name = "NetCore-MCP-Client",
                        version = "1.0.0"
                    }
                }
            };

            await SendRequestAsync(initRequest);

            // 读取初始化响应
            var response = await ReadResponseAsync();
            Console.WriteLine($"初始化响应: {response}");

            // 发送initialized通知
            var initializedNotification = new
            {
                jsonrpc = "2.0",
                method = "notifications/initialized"
            };

            await SendRequestAsync(initializedNotification);
        }

        public async Task<string> ListToolsAsync()
        {
            var request = new
            {
                jsonrpc = "2.0",
                id = requestId++,
                method = "tools/list"
            };

            await SendRequestAsync(request);
            return await ReadResponseAsync();
        }

        public async Task<string> CallToolAsync(string toolName, object arguments)
        {
            var request = new
            {
                jsonrpc = "2.0",
                id = requestId++,
                method = "tools/call",
                @params = new
                {
                    name = toolName,
                    arguments = arguments
                }
            };

            await SendRequestAsync(request);
            return await ReadResponseAsync();
        }

        public async Task<string> ResolveLibraryIdAsync(string libraryName)
        {
            // 首先解析库ID
            var arguments = new
            {
                libraryName = libraryName
            };

            return await CallToolAsync("resolve-library-id", arguments);
        }

        public async Task<string> GetLibraryDocsAsync(string context7CompatibleLibraryID, string topic = null, int tokens = 10000)
        {
            // 获取库文档
            var arguments = new
            {
                context7CompatibleLibraryID = context7CompatibleLibraryID,
                topic = topic,
                tokens = tokens
            };

            return await CallToolAsync("get-library-docs", arguments);
        }

        public async Task<string> SearchLibraryDocumentationAsync(string libraryName, string topic = null)
        {
            try
            {
                Console.WriteLine($"正在解析库ID: {libraryName}");

                // 第一步：解析库ID
                var resolveResult = await ResolveLibraryIdAsync(libraryName);
                Console.WriteLine($"解析结果: {resolveResult}");

                // 从响应中提取库ID（需要解析JSON）
                var resolveResponse = JsonSerializer.Deserialize<JsonElement>(resolveResult);

                if (resolveResponse.TryGetProperty("result", out var result))
                {
                    // 假设返回的结果中包含库ID信息
                    var libraryId = ExtractLibraryIdFromResult(result);

                    if (!string.IsNullOrEmpty(libraryId))
                    {
                        Console.WriteLine($"找到库ID: {libraryId}");

                        // 第二步：获取文档
                        return await GetLibraryDocsAsync(libraryId, topic);
                    }
                }

                return "未找到匹配的库";
            }
            catch (Exception ex)
            {
                return $"搜索失败: {ex.Message}";
            }
        }

        private string ExtractLibraryIdFromResult(JsonElement result)
        {
            try
            {
                // 打印原始结果以便调试
                Console.WriteLine($"原始resolve结果: {result.GetRawText()}");

                if (result.ValueKind == JsonValueKind.Object)
                {
                    // 查找content数组
                    if (result.TryGetProperty("content", out var contentElement) &&
                        contentElement.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in contentElement.EnumerateArray())
                        {
                            if (item.TryGetProperty("type", out var typeElement) &&
                                typeElement.GetString() == "text" &&
                                item.TryGetProperty("text", out var textElement))
                            {
                                var text = textElement.GetString();
                                Console.WriteLine($"resolve返回文本: {text}");

                                // 从文本中提取库ID
                                var libraryId = ExtractLibraryIdFromText(text);
                                if (!string.IsNullOrEmpty(libraryId))
                                {
                                    return libraryId;
                                }
                            }
                        }
                    }

                    // 尝试其他可能的属性
                    if (result.TryGetProperty("libraryId", out var idElement))
                    {
                        return idElement.GetString();
                    }

                    if (result.TryGetProperty("selectedLibrary", out var selectedElement) &&
                        selectedElement.TryGetProperty("id", out var selectedId))
                    {
                        return selectedId.GetString();
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"解析库ID时出错: {ex.Message}");
                return null;
            }
        }

        private string ExtractLibraryIdFromText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            // 查找常见的库ID格式模式
            var patterns = new[]
            {
                @"Library ID:\s*([^\s\n]+)",
                @"Selected library:\s*([^\s\n]+)",
                @"ID:\s*([^\s\n]+)",
                @"Library:\s*([^\s\n]+)",
                @"([a-zA-Z0-9-_]+/[a-zA-Z0-9-_]+)", // 匹配 org/repo 格式
            };

            foreach (var pattern in patterns)
            {
                var match = System.Text.RegularExpressions.Regex.Match(text, pattern);
                if (match.Success && match.Groups.Count > 1)
                {
                    var libraryId = match.Groups[1].Value.Trim();
                    Console.WriteLine($"提取到库ID: {libraryId}");
                    return libraryId;
                }
            }

            // 如果没有匹配到模式，询问用户
            Console.WriteLine($"无法自动解析库ID，请从以下文本中手动输入库ID:");
            Console.WriteLine(text);
            Console.Write("请输入库ID (格式如 microsoft/dotnet): ");
            return Console.ReadLine()?.Trim();
        }

        private async Task SendRequestAsync(object request)
        {
            var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            Console.WriteLine($"发送请求: {json}");
            await stdin.WriteLineAsync(json);
            await stdin.FlushAsync();
        }

        private async Task<string> ReadResponseAsync()
        {
            try
            {
                var response = await stdout.ReadLineAsync();
                Console.WriteLine($"收到响应: {response}");
                return response ?? "";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"读取响应失败: {ex.Message}");
                return "";
            }
        }

        public void Dispose()
        {
            stdin?.Close();
            stdout?.Close();
            mcpProcess?.Kill();
            mcpProcess?.Dispose();
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("启动Context7 MCP客户端...");

            var client = new Context7Client();

            try
            {
                // 连接到MCP服务器
                if (!await client.ConnectAsync())
                {
                    Console.WriteLine("连接失败");
                    return;
                }

                Console.WriteLine("连接成功！");

                // 列出可用工具
                Console.WriteLine("\n=== 列出可用工具 ===");
                var tools = await client.ListToolsAsync();
                Console.WriteLine(tools);

                // 测试单独的resolve步骤
                Console.WriteLine("\n=== 测试库ID解析 ===");
                var testLibraries = new[] { "dotnet", "react", "vue", "express" };

                foreach (var lib in testLibraries)
                {
                    Console.WriteLine($"\n测试库: {lib}");
                    var resolveResult = await client.ResolveLibraryIdAsync(lib);
                    Console.WriteLine($"解析结果: {resolveResult}");
                    Console.WriteLine(new string('-', 50));
                }

                // 手动测试完整流程
                Console.WriteLine("\n=== 手动测试完整流程 ===");
                Console.Write("输入要查询的库名称: ");
                var manualLib = Console.ReadLine();

                if (!string.IsNullOrEmpty(manualLib))
                {
                    // 步骤1：解析库ID
                    Console.WriteLine($"\n步骤1: 解析 '{manualLib}' 的库ID");
                    var resolveResponse = await client.ResolveLibraryIdAsync(manualLib);
                    Console.WriteLine($"解析响应: {resolveResponse}");

                    // 步骤2：手动输入库ID（如果自动解析失败）
                    Console.Write("\n请输入从上面响应中找到的库ID (格式如 microsoft/dotnet): ");
                    var manualLibraryId = Console.ReadLine();

                    if (!string.IsNullOrEmpty(manualLibraryId))
                    {
                        Console.Write("输入具体主题 (可选): ");
                        var manualTopic = Console.ReadLine();

                        Console.WriteLine($"\n步骤2: 获取 '{manualLibraryId}' 的文档");
                        var docsResponse = await client.GetLibraryDocsAsync(manualLibraryId, manualTopic);
                        Console.WriteLine($"文档响应: {docsResponse}");
                    }
                }

                // 演示如何使用Context7
                Console.WriteLine("\n=== Context7自动化查询 ===");

                // 示例1：查询.NET Core相关文档
                var netCoreResult = await client.SearchLibraryDocumentationAsync("dotnet", "getting started");
                Console.WriteLine($"查询结果: {netCoreResult}");

                // 持续交互
                Console.WriteLine("\n=== 持续交互模式 ===");
                Console.WriteLine("输入库名称 (如: react, vue, express, dotnet) 或 'quit' 退出");

                while (true)
                {
                    Console.Write("\n输入要查询的库名称: ");
                    var libraryName = Console.ReadLine();

                    if (libraryName?.ToLower() == "quit")
                        break;

                    if (!string.IsNullOrEmpty(libraryName))
                    {
                        Console.Write("输入具体主题 (可选，直接回车跳过): ");
                        var topic = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(topic))
                            topic = null;

                        var result = await client.SearchLibraryDocumentationAsync(libraryName, topic);
                        Console.WriteLine($"查询结果: {result}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"错误: {ex.Message}");
            }
            finally
            {
                client.Dispose();
                Console.WriteLine("客户端已关闭");
            }
        }
    }
}