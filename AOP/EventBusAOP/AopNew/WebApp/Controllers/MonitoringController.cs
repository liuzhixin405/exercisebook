using Common.Bus.Core;
using Common.Bus.Monitoring;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace WebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MonitoringController : ControllerBase
    {
        private readonly IMetricsCollector _metricsCollector;
        private readonly ILogger<MonitoringController> _logger;

        public MonitoringController(IMetricsCollector metricsCollector, ILogger<MonitoringController> logger)
        {
            _metricsCollector = metricsCollector;
            _logger = logger;
        }

        /// <summary>
        /// 获取当前指标
        /// </summary>
        [HttpGet("metrics")]
        public IActionResult GetMetrics()
        {
            try
            {
                var metrics = _metricsCollector.GetCurrentMetrics();
                return Ok(new
                {
                    Timestamp = DateTime.UtcNow,
                    Metrics = new
                    {
                        ProcessedCommands = metrics.ProcessedCommands,
                        FailedCommands = metrics.FailedCommands,
                        SuccessRate = metrics.SuccessRate,
                        AverageProcessingTime = metrics.AverageProcessingTime.TotalMilliseconds,
                        TotalProcessingTime = metrics.TotalProcessingTime.TotalMilliseconds,
                        AvailableConcurrency = metrics.AvailableConcurrency,
                        MaxConcurrency = metrics.MaxConcurrency,
                        InputQueueSize = metrics.InputQueueSize
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get metrics");
                return BadRequest(new { Error = ex.Message });
            }
        }

        /// <summary>
        /// 重置指标
        /// </summary>
        [HttpPost("metrics/reset")]
        public IActionResult ResetMetrics()
        {
            try
            {
                _metricsCollector.ResetMetrics();
                return Ok(new { Message = "Metrics reset successfully", Timestamp = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reset metrics");
                return BadRequest(new { Error = ex.Message });
            }
        }

        /// <summary>
        /// SSE实时监控流
        /// </summary>
        [HttpGet("stream")]
        public async Task StreamMetrics()
        {
            Response.Headers.Add("Content-Type", "text/event-stream");
            Response.Headers.Add("Cache-Control", "no-cache");
            Response.Headers.Add("Connection", "keep-alive");
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            Response.Headers.Add("Access-Control-Allow-Headers", "Cache-Control");

            var clientId = Guid.NewGuid().ToString();
            _logger.LogInformation("SSE client {ClientId} connected", clientId);

            // 订阅指标更新事件
            EventHandler<MetricsUpdatedEventArgs>? handler = null;
            
            try
            {
                // 发送初始连接确认
                await WriteSSEMessage("connected", new { ClientId = clientId, Timestamp = DateTime.UtcNow });

                handler = async (sender, e) =>
                {
                    try
                    {
                        var data = new
                        {
                            Timestamp = e.Timestamp,
                            Metrics = new
                            {
                                ProcessedCommands = e.Metrics.ProcessedCommands,
                                FailedCommands = e.Metrics.FailedCommands,
                                SuccessRate = e.Metrics.SuccessRate,
                                AverageProcessingTime = e.Metrics.AverageProcessingTime.TotalMilliseconds,
                                TotalProcessingTime = e.Metrics.TotalProcessingTime.TotalMilliseconds,
                                AvailableConcurrency = e.Metrics.AvailableConcurrency,
                                MaxConcurrency = e.Metrics.MaxConcurrency,
                                InputQueueSize = e.Metrics.InputQueueSize
                            }
                        };

                        await WriteSSEMessage("metrics", data);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error sending metrics to client {ClientId}", clientId);
                    }
                };

                _metricsCollector.MetricsUpdated += handler;

                // 开始收集指标
                _metricsCollector.StartCollecting();

                // 保持连接活跃
                while (!HttpContext.RequestAborted.IsCancellationRequested)
                {
                    await WriteSSEMessage("heartbeat", new { Timestamp = DateTime.UtcNow });
                    await Task.Delay(30000, HttpContext.RequestAborted); // 每30秒发送心跳
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("SSE client {ClientId} disconnected", clientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SSE stream for client {ClientId}", clientId);
                await WriteSSEMessage("error", new { Error = ex.Message, Timestamp = DateTime.UtcNow });
            }
            finally
            {
                if (handler != null)
                {
                    _metricsCollector.MetricsUpdated -= handler;
                }
                _metricsCollector.StopCollecting();
            }
        }

        /// <summary>
        /// 获取监控页面
        /// </summary>
        [HttpGet("dashboard")]
        public IActionResult GetDashboard()
        {
            var html = @"
<!DOCTYPE html>
<html>
<head>
    <title>Dataflow CommandBus 实时监控</title>
    <meta charset='utf-8'>
    <style>
        body { font-family: Arial, sans-serif; margin: 20px; background-color: #f5f5f5; }
        .container { max-width: 1200px; margin: 0 auto; }
        .header { background: #2c3e50; color: white; padding: 20px; border-radius: 8px; margin-bottom: 20px; }
        .metrics-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(250px, 1fr)); gap: 20px; }
        .metric-card { background: white; padding: 20px; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }
        .metric-value { font-size: 2em; font-weight: bold; color: #2c3e50; }
        .metric-label { color: #7f8c8d; margin-top: 5px; }
        .status { padding: 10px; border-radius: 4px; margin-bottom: 20px; }
        .status.connected { background: #d4edda; color: #155724; border: 1px solid #c3e6cb; }
        .status.disconnected { background: #f8d7da; color: #721c24; border: 1px solid #f5c6cb; }
        .chart-container { background: white; padding: 20px; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); margin-top: 20px; }
        .controls { margin-bottom: 20px; }
        .btn { padding: 10px 20px; margin: 5px; border: none; border-radius: 4px; cursor: pointer; }
        .btn-primary { background: #007bff; color: white; }
        .btn-danger { background: #dc3545; color: white; }
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🚀 Dataflow CommandBus 实时监控</h1>
            <p>基于TPL数据流的高性能命令处理系统监控面板</p>
        </div>
        
        <div id='status' class='status disconnected'>
            🔴 未连接 - 正在尝试连接...
        </div>
        
        <div class='controls'>
            <button class='btn btn-primary' onclick='connect()'>连接</button>
            <button class='btn btn-danger' onclick='disconnect()'>断开</button>
            <button class='btn btn-primary' onclick='resetMetrics()'>重置指标</button>
        </div>
        
        <div class='metrics-grid'>
            <div class='metric-card'>
                <div class='metric-value' id='processedCommands'>0</div>
                <div class='metric-label'>已处理命令数</div>
            </div>
            <div class='metric-card'>
                <div class='metric-value' id='failedCommands'>0</div>
                <div class='metric-label'>失败命令数</div>
            </div>
            <div class='metric-card'>
                <div class='metric-value' id='successRate'>0%</div>
                <div class='metric-label'>成功率</div>
            </div>
            <div class='metric-card'>
                <div class='metric-value' id='avgProcessingTime'>0ms</div>
                <div class='metric-label'>平均处理时间</div>
            </div>
            <div class='metric-card'>
                <div class='metric-value' id='availableConcurrency'>0</div>
                <div class='metric-label'>可用并发数</div>
            </div>
            <div class='metric-card'>
                <div class='metric-value' id='inputQueueSize'>0</div>
                <div class='metric-label'>输入队列大小</div>
            </div>
        </div>
        
        <div class='chart-container'>
            <h3>📊 实时性能图表</h3>
            <canvas id='performanceChart' width='800' height='400'></canvas>
        </div>
    </div>

    <script>
        let eventSource = null;
        let isConnected = false;
        let performanceData = [];
        const maxDataPoints = 50;

        function connect() {
            if (eventSource) {
                eventSource.close();
            }
            
            eventSource = new EventSource('/api/Monitoring/stream');
            
            eventSource.onopen = function(event) {
                isConnected = true;
                updateStatus('🟢 已连接 - 实时监控中...', 'connected');
                console.log('SSE连接已建立');
            };
            
            eventSource.onmessage = function(event) {
                console.log('收到消息:', event.data);
            };
            
            eventSource.addEventListener('connected', function(event) {
                const data = JSON.parse(event.data);
                console.log('连接确认:', data);
            });
            
            eventSource.addEventListener('metrics', function(event) {
                const data = JSON.parse(event.data);
                updateMetrics(data.metrics);
                addPerformanceData(data.metrics);
            });
            
            eventSource.addEventListener('heartbeat', function(event) {
                console.log('心跳:', event.data);
            });
            
            eventSource.addEventListener('error', function(event) {
                const data = JSON.parse(event.data);
                console.error('错误:', data);
                updateStatus('🔴 连接错误: ' + data.error, 'disconnected');
            });
            
            eventSource.onerror = function(event) {
                isConnected = false;
                updateStatus('🔴 连接断开 - 正在重连...', 'disconnected');
                console.error('SSE连接错误');
            };
        }
        
        function disconnect() {
            if (eventSource) {
                eventSource.close();
                eventSource = null;
            }
            isConnected = false;
            updateStatus('🔴 已断开连接', 'disconnected');
        }
        
        function updateStatus(message, className) {
            const statusEl = document.getElementById('status');
            statusEl.textContent = message;
            statusEl.className = 'status ' + className;
        }
        
        function updateMetrics(metrics) {
            document.getElementById('processedCommands').textContent = metrics.ProcessedCommands.toLocaleString();
            document.getElementById('failedCommands').textContent = metrics.FailedCommands.toLocaleString();
            document.getElementById('successRate').textContent = metrics.SuccessRate.toFixed(2) + '%';
            document.getElementById('avgProcessingTime').textContent = metrics.AverageProcessingTime.toFixed(2) + 'ms';
            document.getElementById('availableConcurrency').textContent = metrics.AvailableConcurrency;
            document.getElementById('inputQueueSize').textContent = metrics.InputQueueSize;
        }
        
        function addPerformanceData(metrics) {
            performanceData.push({
                timestamp: new Date(),
                processedCommands: metrics.ProcessedCommands,
                successRate: metrics.SuccessRate,
                avgProcessingTime: metrics.AverageProcessingTime
            });
            
            if (performanceData.length > maxDataPoints) {
                performanceData.shift();
            }
            
            updateChart();
        }
        
        function updateChart() {
            const canvas = document.getElementById('performanceChart');
            const ctx = canvas.getContext('2d');
            
            // 简单的图表绘制
            ctx.clearRect(0, 0, canvas.width, canvas.height);
            
            if (performanceData.length < 2) return;
            
            const padding = 40;
            const chartWidth = canvas.width - 2 * padding;
            const chartHeight = canvas.height - 2 * padding;
            
            // 绘制坐标轴
            ctx.strokeStyle = '#ddd';
            ctx.lineWidth = 1;
            ctx.beginPath();
            ctx.moveTo(padding, padding);
            ctx.lineTo(padding, canvas.height - padding);
            ctx.lineTo(canvas.width - padding, canvas.height - padding);
            ctx.stroke();
            
            // 绘制成功率曲线
            ctx.strokeStyle = '#28a745';
            ctx.lineWidth = 2;
            ctx.beginPath();
            
            for (let i = 0; i < performanceData.length; i++) {
                const x = padding + (i / (performanceData.length - 1)) * chartWidth;
                const y = canvas.height - padding - (performanceData[i].successRate / 100) * chartHeight;
                
                if (i === 0) {
                    ctx.moveTo(x, y);
                } else {
                    ctx.lineTo(x, y);
                }
            }
            ctx.stroke();
            
            // 绘制图例
            ctx.fillStyle = '#28a745';
            ctx.fillRect(canvas.width - 120, 20, 15, 15);
            ctx.fillStyle = '#333';
            ctx.font = '12px Arial';
            ctx.fillText('成功率 (%)', canvas.width - 100, 32);
        }
        
        function resetMetrics() {
            fetch('/api/Monitoring/metrics/reset', { method: 'POST' })
                .then(response => response.json())
                .then(data => {
                    console.log('指标重置成功:', data);
                    alert('指标已重置');
                })
                .catch(error => {
                    console.error('重置指标失败:', error);
                    alert('重置指标失败: ' + error.message);
                });
        }
        
        // 页面加载时自动连接
        window.onload = function() {
            connect();
        };
        
        // 页面卸载时断开连接
        window.onbeforeunload = function() {
            disconnect();
        };
    </script>
</body>
</html>";
            
            return Content(html, "text/html");
        }

        private async Task WriteSSEMessage(string eventType, object data)
        {
            try
            {
                var json = JsonSerializer.Serialize(data);
                var message = $"event: {eventType}\ndata: {json}\n\n";
                var bytes = Encoding.UTF8.GetBytes(message);
                
                await Response.Body.WriteAsync(bytes, 0, bytes.Length);
                await Response.Body.FlushAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error writing SSE message");
            }
        }
    }
}
