using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class SSEController : ControllerBase
{
    [HttpGet("events")]
    public async Task GetEvents()
    {
        // 设置响应头，声明是 SSE 流
        Response.ContentType = "text/event-stream";
        Response.Headers.Add("Cache-Control", "no-cache");
        Response.Headers.Add("Connection", "keep-alive");

        var tokens = new[] { "this", "is", "a", "big", "stream", "for", "event", "stream", "test" };

        await using var writer = new StreamWriter(Response.Body, Encoding.UTF8, leaveOpen: true);

        foreach (var token in tokens)
        {
            // 写入事件数据（注意换行符）
            await writer.WriteLineAsync($"data: {token}\n");
            await writer.FlushAsync(); // 确保立即推送数据
            await Task.Delay(420); // 模拟延迟
        }
    }
}
