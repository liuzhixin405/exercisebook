using System.Net.WebSockets;

namespace WebSocketServer.Snippets
{
    public static class Program
    {
        public static void UseWebSockets(WebApplication app)
        {
            app.UseWebSockets();
        }


        public static void AcceptWebSocketAsync(WebApplication app)
        {
            app.Use(async (context, next) => {
                if (context.Request.Path.Equals("ws"))
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        await Echo(webSocket);
                    }
                    else
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    }
                }
                else
                {
                    await next(context);
                }
            });
        }

        public static void AcceptWebSocketAsyncBackgroundSocketProcessor(WebApplication app)
        {
            app.Run(async (context) => { 
            
                using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                var socketFinishedTcs = new TaskCompletionSource<object>();

                BackgroundSocketProcessor.AddSocket(webSocket, socketFinishedTcs);

                await socketFinishedTcs.Task;
            
            });
        }

        public static void UseWebSocketOptionsAllowedOrigins(WebApplication app)
        {
            var webSocketOptions = new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromMilliseconds(2),
            };
            webSocketOptions.AllowedOrigins.Add("https://*");
        }
        private static async Task Echo(WebSocket socket) //websocket 主要accept链接 receive消息 和close链接
        {
            var buffer = new byte[1024 * 4];
            var receiveResult = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!receiveResult.CloseStatus.HasValue)
            {
                await socket.SendAsync(
                    new ArraySegment<byte>(buffer, 0, receiveResult.Count),receiveResult.MessageType,receiveResult.EndOfMessage,CancellationToken.None
                    );
                receiveResult = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await socket.CloseAsync(receiveResult.CloseStatus.Value,receiveResult.CloseStatusDescription,CancellationToken.None);
        }
    }
}
