using MCPHostApp;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

// 创建 HttpClient 和基本配置
HttpClient httpClient = new();
string ollamaEndpoint = "http://localhost:11434/api/chat";  // 修正这里
string modelName = "qwen2.5-coder:7b"; // 或者 llama3, phi3 等

// 创建 MCP 客户端连接（启动 MCP Server）
IMcpClient mcpClient = await McpClientFactory.CreateAsync(
    new StdioClientTransport(new()
    {
        Command = "dotnet",
        Arguments = ["run", "--project", "G:\\github\\exercisebook\\AI\\MinimalMcpServer\\MinimalMcpServer.csproj"],
        Name = "Minimal MCP Server",
    }));

// 获取工具列表
Console.WriteLine("Available tools:");
var tools = await mcpClient.ListToolsAsync();
foreach (var tool in tools)
{
    Console.WriteLine(tool);
}
Console.WriteLine();

// 聊天主循环
var messages = new List<ChatMessage>();
while (true)
{
    Console.Write("Prompt: ");
    var userInput = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(userInput)) break;

    
    messages.Add(new ChatMessage(ChatRole.User, userInput));

    var request = new
    {
        model = modelName,
        messages = messages,
        stream = false,
        // 不传 tools
        tools = tools.Select(t => t.ToOpenAIToolObject()).ToList(),
    };

    var response = await httpClient.PostAsJsonAsync(ollamaEndpoint, request);
    var resultJson = await response.Content.ReadAsStringAsync();
    using var doc = JsonDocument.Parse(resultJson);
    //Console.WriteLine(doc.RootElement.ToString());
    var messageContent = doc.RootElement
       //.GetProperty("choices")
       //[0]
       .GetProperty("message")
       .GetProperty("content")
       .GetString();

    Console.WriteLine($"Assistant: {messageContent}");
    messages.Add(new ChatMessage(ChatRole.Assistant, messageContent));
    
}
