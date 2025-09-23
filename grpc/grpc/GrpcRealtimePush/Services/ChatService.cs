using Grpc.Core;

namespace GrpcRealtimePush.Services;

public class ChatService : GrpcRealtimePush.ChatService.ChatServiceBase
{
    private readonly ILogger<ChatService> _logger;

    public ChatService(ILogger<ChatService> logger)
    {
        _logger = logger;
    }



    public override async Task StartRealtimePush(RealtimePushRequest request, 
        IServerStreamWriter<RealtimePushResponse> responseStream, ServerCallContext context)
    {
        _logger.LogInformation("🚀 实时推送已启动! 客户端: {ClientId}", request.ClientId);
        
        try
        {
            // Start continuous data push
            var counter = 1;
            var random = new Random();
            var dataTypes = new[] { "系统状态", "用户活动", "数据更新", "通知消息", "性能指标" };
            
            _logger.LogInformation("🔄 开始连续数据推送循环...");
            
            while (!context.CancellationToken.IsCancellationRequested && counter <= 100)
            {
                // Simulate different types of real-time data
                var dataType = dataTypes[random.Next(dataTypes.Length)];
                var value = random.Next(1, 1000);
                var timestamp = DateTime.UtcNow;
                
                var response = new RealtimePushResponse
                {
                    Data = $"#{counter:D4} - 数值: {value} | 时间: {timestamp:HH:mm:ss.fff}",
                    Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    DataType = dataType
                };

                await responseStream.WriteAsync(response);
                _logger.LogInformation("📡 推送数据 #{Counter}: [{DataType}] = {Value} at {Time}", 
                    counter, dataType, value, timestamp.ToString("HH:mm:ss.fff"));
                
                counter++;
                
                // Wait before sending next update
                await Task.Delay(2000, context.CancellationToken);
            }
            
            // Send final message
            await responseStream.WriteAsync(new RealtimePushResponse
            {
                Data = "实时推送测试完成！",
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                DataType = "系统消息"
            });
            
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("实时推送会话已取消，客户端: {ClientId}", request.ClientId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "实时推送会话出错: {Error}", ex.Message);
            
            // Try to send error message to client
            try
            {
                await responseStream.WriteAsync(new RealtimePushResponse
                {
                    Data = $"服务器错误: {ex.Message}",
                    Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    DataType = "错误消息"
                });
            }
            catch (Exception sendError)
            {
                _logger.LogError(sendError, "发送错误消息失败");
            }
        }
        
        _logger.LogInformation("实时推送会话结束，客户端: {ClientId}", request.ClientId);
    }


}