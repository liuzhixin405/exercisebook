using Framework.Infrastructure.Decorators;
using System.Diagnostics;

namespace Framework.Samples.Decorators;

/// <summary>
/// 性能监控器实现 - 装饰器模式示例
/// </summary>
public class PerformanceMonitor : IPerformanceMonitor
{
    public IDisposable StartMonitoring(string operationName)
    {
        return new PerformanceMonitoringContext(this, operationName);
    }

    public void RecordMetric(string operationName, TimeSpan duration)
    {
        Console.WriteLine($"[性能监控] 操作: {operationName}, 耗时: {duration.TotalMilliseconds:F2}ms");
    }

    private class PerformanceMonitoringContext : IDisposable
    {
        private readonly PerformanceMonitor _monitor;
        private readonly string _operationName;
        private readonly Stopwatch _stopwatch;

        public PerformanceMonitoringContext(PerformanceMonitor monitor, string operationName)
        {
            _monitor = monitor;
            _operationName = operationName;
            _stopwatch = Stopwatch.StartNew();
            Console.WriteLine($"[性能监控] 开始监控: {operationName}");
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            _monitor.RecordMetric(_operationName, _stopwatch.Elapsed);
        }
    }
}
