using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

var model = "qwen2.5-coder:7b"; // 替换为你的本地模型名称
var ollamaEndpoint = "http://localhost:11434/api/chat";

List<Message> messages = new()
{
    new Message
    {
        Role = "system",
        Content = """
            You are a professional and friendly programming assistant who helps developers solve coding problems, write clean and efficient code, and understand technical concepts.

            When answering questions:
            - Prefer code examples with clear explanations.
            - Ask for clarification if the question is vague.
            - Avoid hallucinations—if you're not sure, say so.
            - When possible, provide best practices and alternatives.

            You support multiple programming languages (like C#, Python, JavaScript, etc.), popular frameworks (ASP.NET, React, Django, etc.), and tools (Git, Docker, etc.).

            At the end of your response, ask if the developer needs further help.
        """
    }
};

var handler = new HttpClientHandler
{
    AllowAutoRedirect = false
};
var httpClient = new HttpClient(handler);

while (true)
{
    Console.Write("Your prompt: ");
    var userInput = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(userInput)) break;

    messages.Add(new Message { Role = "user", Content = userInput });

    Console.Write("AI Response: ");

    var request = new
    {
        model = model,
        messages = messages,
        stream = true
    };
    var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

using var requestMessage = new HttpRequestMessage(HttpMethod.Post, ollamaEndpoint)
{
    Content = content
};

// 这里添加 Accept 请求头，告诉服务器我们想用 SSE 流式响应
requestMessage.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/event-stream"));

using var response = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);

    response.EnsureSuccessStatusCode();

    using var stream = await response.Content.ReadAsStreamAsync();
    using var reader = new StreamReader(stream);

    var sb = new StringBuilder();
    string? bufferLine;
   
while (!reader.EndOfStream)
{
    var line = await reader.ReadLineAsync();
    if (string.IsNullOrWhiteSpace(line)) continue;
    if (line.StartsWith("data: "))
        line = line[6..];

    var chunk = JsonSerializer.Deserialize<OllamaStreamChunk>(line);
    if (chunk?.Message?.Content != null)
    {
        Console.Write(chunk.Message.Content);
        Console.Out.Flush();
        sb.Append(chunk.Message.Content);
    }
}

    messages.Add(new Message
    {
        Role = "assistant",
        Content = sb.ToString()
    });

    Console.WriteLine();
}

// -- Data classes
public class Message
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = default!;
    [JsonPropertyName("content")]
    public string Content { get; set; } = default!;
}

public class OllamaStreamChunk
{
    [JsonPropertyName("message")]
    public Message? Message { get; set; }

    [JsonPropertyName("done")]
    public bool Done { get; set; }
}
