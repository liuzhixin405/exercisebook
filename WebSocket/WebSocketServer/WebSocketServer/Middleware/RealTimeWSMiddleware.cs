using System.Net.WebSockets;

namespace WebSocketServer.Middleware
{
    public class RealTimeWSMiddleware
    {


        private readonly RequestDelegate _next;
        private WebSocketManager _wSConnectionManager { get; set; }
        private WSHandler _wsHanlder { get; set; }

        public RealTimeWSMiddleware(
            RequestDelegate next,
            WebSocketManager wSConnectionManager,
            WSHandler wsHandler)
        {
            _next = next;
            _wSConnectionManager = wSConnectionManager;
            _wsHanlder = wsHandler;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.WebSockets.IsWebSocketRequest)
            {
                var cancellationToken = httpContext.RequestAborted;
                var currentWebSocket = await httpContext.WebSockets.AcceptWebSocketAsync();
                _wSConnectionManager.AddSocket(currentWebSocket);

                while (true)
                {
                    if (cancellationToken.IsCancellationRequested) break;
                    var response = await _wsHanlder.RecieveAsync(currentWebSocket, cancellationToken);

                    if (string.IsNullOrEmpty(response) && currentWebSocket.State != WebSocketState.Open) break;

                    foreach (var item in _wSConnectionManager.GetAll())
                    {
                        if (item.Value.State == WebSocketState.Open)
                        {
                            await _wsHanlder.SendMessageAsync(item.Value, response, cancellationToken);
                        }
                        continue;
                    }
                }

                await _wSConnectionManager.RemoveSocket(currentWebSocket);
            }
            else
            {
                await _next(httpContext);
            }
        }
    }
}
