using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Common.Bus
{
    /// <summary>
    /// 时间窗口管理器，用于基于时间的批处理优化
    /// </summary>
    public class TimeWindowManager : IDisposable
    {
        private readonly TimeSpan _windowSize;
        private readonly ConcurrentDictionary<string, List<TimeBasedCommandRequest>> _pendingRequests;
        private readonly Timer _windowTimer;
        private readonly ILogger<TimeWindowManager>? _logger;
        private readonly SemaphoreSlim _processingSemaphore;
        private volatile bool _disposed;

        public TimeWindowManager(TimeSpan windowSize, ILogger<TimeWindowManager>? logger = null)
        {
            _windowSize = windowSize;
            _logger = logger;
            _pendingRequests = new ConcurrentDictionary<string, List<TimeBasedCommandRequest>>();
            _processingSemaphore = new SemaphoreSlim(1, 1);
            
            // 创建定时器，定期处理时间窗口
            _windowTimer = new Timer(ProcessTimeWindows, null, _windowSize, _windowSize);
        }

        /// <summary>
        /// 添加请求到时间窗口
        /// </summary>
        public void AddRequest(TimeBasedCommandRequest request)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(TimeWindowManager));

            var windowId = request.GetTimeWindowId(_windowSize);
            
            _pendingRequests.AddOrUpdate(
                windowId,
                new List<TimeBasedCommandRequest> { request },
                (key, existing) =>
                {
                    lock (existing)
                    {
                        existing.Add(request);
                        return existing;
                    }
                });

            _logger?.LogDebug("Added request {RequestId} to time window {WindowId}", 
                request.RequestId, windowId);
        }

        /// <summary>
        /// 获取指定时间窗口内的所有请求
        /// </summary>
        public List<TimeBasedCommandRequest> GetRequestsInWindow(DateTime windowStart)
        {
            var windowId = windowStart.Ticks.ToString();
            
            if (_pendingRequests.TryRemove(windowId, out var requests))
            {
                lock (requests)
                {
                    return new List<TimeBasedCommandRequest>(requests);
                }
            }
            
            return new List<TimeBasedCommandRequest>();
        }

        /// <summary>
        /// 获取当前时间窗口内的所有请求
        /// </summary>
        public List<TimeBasedCommandRequest> GetCurrentWindowRequests()
        {
            var currentTime = DateTime.UtcNow;
            var windowStart = new DateTime(currentTime.Ticks / _windowSize.Ticks * _windowSize.Ticks);
            return GetRequestsInWindow(windowStart);
        }

        /// <summary>
        /// 获取所有待处理的请求
        /// </summary>
        public List<TimeBasedCommandRequest> GetAllPendingRequests()
        {
            var allRequests = new List<TimeBasedCommandRequest>();
            
            foreach (var kvp in _pendingRequests.ToArray())
            {
                lock (kvp.Value)
                {
                    allRequests.AddRange(kvp.Value);
                }
            }
            
            return allRequests;
        }

        /// <summary>
        /// 获取时间窗口统计信息
        /// </summary>
        public TimeWindowMetrics GetMetrics()
        {
            var allRequests = GetAllPendingRequests();
            var windowCount = _pendingRequests.Count;
            var totalRequests = allRequests.Count;
            
            var oldestRequest = allRequests.OrderBy(r => r.CreatedAt).FirstOrDefault();
            var newestRequest = allRequests.OrderByDescending(r => r.CreatedAt).FirstOrDefault();
            
            return new TimeWindowMetrics
            {
                WindowSize = _windowSize,
                ActiveWindows = windowCount,
                TotalPendingRequests = totalRequests,
                OldestRequestAge = oldestRequest != null ? DateTime.UtcNow - oldestRequest.CreatedAt : TimeSpan.Zero,
                NewestRequestAge = newestRequest != null ? DateTime.UtcNow - newestRequest.CreatedAt : TimeSpan.Zero,
                AverageRequestsPerWindow = windowCount > 0 ? (double)totalRequests / windowCount : 0
            };
        }

        /// <summary>
        /// 清理过期的时间窗口
        /// </summary>
        public void CleanupExpiredWindows(TimeSpan maxAge)
        {
            var cutoffTime = DateTime.UtcNow - maxAge;
            var expiredWindows = new List<string>();
            
            foreach (var kvp in _pendingRequests)
            {
                lock (kvp.Value)
                {
                    if (kvp.Value.Count > 0 && kvp.Value[0].CreatedAt < cutoffTime)
                    {
                        expiredWindows.Add(kvp.Key);
                    }
                }
            }
            
            foreach (var windowId in expiredWindows)
            {
                if (_pendingRequests.TryRemove(windowId, out var requests))
                {
                    lock (requests)
                    {
                        foreach (var request in requests)
                        {
                            request.SetException(new TimeoutException($"Request {request.RequestId} expired in time window {windowId}"));
                        }
                    }
                }
            }
            
            if (expiredWindows.Count > 0)
            {
                _logger?.LogWarning("Cleaned up {Count} expired time windows", expiredWindows.Count);
            }
        }

        /// <summary>
        /// 定时处理时间窗口
        /// </summary>
        private async void ProcessTimeWindows(object? state)
        {
            if (_disposed)
                return;

            try
            {
                await _processingSemaphore.WaitAsync();
                
                // 清理过期窗口
                CleanupExpiredWindows(_windowSize * 3); // 保留3个时间窗口
                
                // 处理到期的窗口
                var currentTime = DateTime.UtcNow;
                var windowStart = new DateTime(currentTime.Ticks / _windowSize.Ticks * _windowSize.Ticks);
                var requests = GetRequestsInWindow(windowStart);
                
                if (requests.Count > 0)
                {
                    _logger?.LogDebug("Processing {Count} requests from time window {WindowId}", 
                        requests.Count, windowStart.Ticks);
                    
                    // 触发窗口处理事件
                    OnTimeWindowReady?.Invoke(requests);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error processing time windows");
            }
            finally
            {
                _processingSemaphore.Release();
            }
        }

        /// <summary>
        /// 时间窗口准备就绪事件
        /// </summary>
        public event Action<List<TimeBasedCommandRequest>>? OnTimeWindowReady;

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            _windowTimer?.Dispose();
            _processingSemaphore?.Dispose();
            
            // 取消所有待处理的请求
            var allRequests = GetAllPendingRequests();
            foreach (var request in allRequests)
            {
                request.SetException(new ObjectDisposedException(nameof(TimeWindowManager)));
            }
            
            _pendingRequests.Clear();
        }
    }

    /// <summary>
    /// 时间窗口统计信息
    /// </summary>
    public class TimeWindowMetrics
    {
        public TimeSpan WindowSize { get; set; }
        public int ActiveWindows { get; set; }
        public int TotalPendingRequests { get; set; }
        public TimeSpan OldestRequestAge { get; set; }
        public TimeSpan NewestRequestAge { get; set; }
        public double AverageRequestsPerWindow { get; set; }
    }
}
