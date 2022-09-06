using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace WebSocketServer.Middleware
{
    public class WebSocketManager
    {
        private static ConcurrentDictionary<string,WebSocket> _sockeDic = new ConcurrentDictionary<string, WebSocket>();

        public void AddSocket(WebSocket webSocket)
        {
            _sockeDic.TryAdd(Guid.NewGuid().ToString(), webSocket);
        }

        public async Task RemoveSocket(WebSocket socket)
        {
            _sockeDic.TryRemove(GetSocketId(socket), out WebSocket aSocket);

            await aSocket.CloseAsync(
                closeStatus: WebSocketCloseStatus.NormalClosure,
                statusDescription: "Close by User",
                cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }
        public string GetSocketId(WebSocket socket)
        {
            return _sockeDic.FirstOrDefault(k => k.Value == socket).Key;
        }

        public ConcurrentDictionary<string, WebSocket> GetAll()
        {
            return _sockeDic;
        }

    }
}
