using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;

public class NetworkUsage
{
    public string GetNetworkUsage()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return GetWindowsNetworkUsage();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return GetLinuxNetworkUsage();
        }
        else
        {
            return "Unsupported operating system.";
        }
    }

    private string GetWindowsNetworkUsage()
    {
        try
        {
            // 获取 PerformanceCounter 支持的所有网络接口实例
            var category = new PerformanceCounterCategory("Network Interface");
            var validInstances = category.GetInstanceNames(); // 返回支持的实例名称

            // 获取系统中活动的网络接口
            var interfaces = NetworkInterface.GetAllNetworkInterfaces()
                .Where(ni => ni.OperationalStatus == OperationalStatus.Up
                             && validInstances.Contains(ni.Description)) // 匹配实例名称
                .ToList();

            if (!interfaces.Any())
            {
                return "No valid network interfaces found.";
            }

            var result = new StringBuilder();

            foreach (var iface in interfaces)
            {
                try
                {
                    var networkIn = new PerformanceCounter("Network Interface", "Bytes Received/sec", iface.Description);
                    var networkOut = new PerformanceCounter("Network Interface", "Bytes Sent/sec", iface.Description);

                    var receivedBytes = networkIn.NextValue() / (1024 * 1024); // 转换为 MB
                    var sentBytes = networkOut.NextValue() / (1024 * 1024); // 转换为 MB

                    result.AppendLine($"{iface.Name} ({iface.Description}): {receivedBytes:F2} MB received, {sentBytes:F2} MB sent per second");
                }
                catch (Exception ex)
                {
                    result.AppendLine($"Error retrieving data for {iface.Name} ({iface.Description}): {ex.Message}");
                }
            }

            return result.ToString();
        }
        catch (Exception ex)
        {
            return $"Error retrieving network usage on Windows: {ex.Message}";
        }
    }



    private string GetLinuxNetworkUsage()
    {
        try
        {
            if (!File.Exists("/proc/net/dev"))
                return "Unable to access network statistics (Linux only)";

            string[] lines = File.ReadAllLines("/proc/net/dev");

            var networkInterfaces = lines
                .Skip(2) // 跳过前两行标题
                .Select(line => line.Trim())
                .Where(line => line.Contains(":"))
                .Select(ParseNetworkLine)
                .ToList();

            return string.Join("\n", networkInterfaces.Select(ni =>
                $"{ni.Interface}: {ni.ReceivedMB:F2} MB received, {ni.TransmittedMB:F2} MB sent"));
        }
        catch (Exception ex)
        {
            return $"Error retrieving network usage on Linux: {ex.Message}";
        }
    }

    private (string Interface, double ReceivedMB, double TransmittedMB) ParseNetworkLine(string line)
    {
        var parts = line.Split(new[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);
        string interfaceName = parts[0];

        long receivedBytes = long.Parse(parts[1]);   // 接收字节
        long transmittedBytes = long.Parse(parts[9]); // 发送字节

        return (
            Interface: interfaceName,
            ReceivedMB: receivedBytes / (1024.0 * 1024.0),   // 转换为 MB
            TransmittedMB: transmittedBytes / (1024.0 * 1024.0) // 转换为 MB
        );
    }
}
