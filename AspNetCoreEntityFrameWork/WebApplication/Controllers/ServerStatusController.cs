using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Runtime.InteropServices;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServerStatusController : ControllerBase
    {
        // 定义性能计数器来获取 CPU 使用率
        private readonly PerformanceCounter _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

        [HttpGet("status")]
        public async Task GetServerStatus()
        {
            // 设置响应头，声明是 SSE 流
            Response.ContentType = "text/event-stream";
            Response.Headers.Add("Cache-Control", "no-cache");
            Response.Headers.Add("Connection", "keep-alive");

            // 获取当前进程的基本信息
            var process = Process.GetCurrentProcess();

            await using var writer = new StreamWriter(Response.Body, Encoding.UTF8, leaveOpen: true);

            while (!HttpContext.RequestAborted.IsCancellationRequested)
            {
                // 获取 CPU 使用率
                var cpuUsage = _cpuCounter.NextValue(); // CPU 使用率百分比
                var memoryUsage = process.WorkingSet64 / (1024 * 1024); // 内存使用（MB）
                var uptime = (DateTime.Now - process.StartTime).ToString(@"hh\:mm\:ss"); // 服务器运行时间

                // 获取系统的磁盘使用情况
                var diskUsage = GetDiskUsage();

                // 获取系统的网络使用情况（假设 Windows 上可用）
                var networkUsage = new NetworkUsage().GetNetworkUsage();

                // 构建状态信息
                var status = new
                {
                    CPU = $"{cpuUsage:F2}%",
                    Memory = $"{memoryUsage} MB",
                    Uptime = uptime,
                    DiskUsage = diskUsage,
                    NetworkUsage = networkUsage,
                    Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };

                // 将状态信息转化为 JSON 格式并发送
                await writer.WriteLineAsync($"data: {System.Text.Json.JsonSerializer.Serialize(status)}\n");
                await writer.FlushAsync(); // 确保立即推送数据
                await Task.Delay(1000*2); // 每秒更新一次
            }
        }

        // 获取磁盘使用情况（Windows）
        private string GetDiskUsage()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var drive = DriveInfo.GetDrives().FirstOrDefault(d => d.IsReady);
                if (drive != null)
                {
                    return $"{drive.TotalFreeSpace / (1024 * 1024 * 1024)} GB free of {drive.TotalSize / (1024 * 1024 * 1024)} GB";
                }
            }
            return "N/A";
        }

    }
}
