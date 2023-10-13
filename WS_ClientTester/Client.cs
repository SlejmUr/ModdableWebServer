using NetCoreServer;
using System.Net.Sockets;
using System.Text;

namespace WS_ClientTester
{
    public class Client : WsClient
    {
        public Client(string address, int port) : base(address, port) { }

        public void DisconnectAndStop()
        {
            _stop = true;
            CloseAsync(1000);
            while (IsConnected)
                Thread.Yield();
        }

        public override void OnWsConnecting(HttpRequest request)
        {
            request.SetBegin("GET", $"/ws/{Id}");
            request.SetHeader("Host", "localhost");
            request.SetHeader("Origin", "http://localhost");
            request.SetHeader("Upgrade", "websocket");
            request.SetHeader("Connection", "Upgrade");
            request.SetHeader("Sec-WebSocket-Key", Convert.ToBase64String(WsNonce));
            request.SetHeader("Sec-WebSocket-Protocol", "chat, superchat");
            request.SetHeader("Sec-WebSocket-Version", "13");
            request.SetBody();
        }

        public override void OnWsConnected(HttpResponse response)
        {
            Console.WriteLine($"Chat WebSocket client connected a new session with Id {Id}");
        }

        public override void OnWsDisconnected()
        {
            Console.WriteLine($"Chat WebSocket client disconnected a session with Id {Id}");
        }

        public override void OnWsReceived(byte[] buffer, long offset, long size)
        {
            Console.WriteLine($"Incoming: {Encoding.UTF8.GetString(buffer, (int)offset, (int)size)}");
        }

        protected override void OnDisconnected()
        {
            base.OnDisconnected();

            Console.WriteLine($"Chat WebSocket client disconnected a session with Id {Id}");

            // Wait for a while...
            Thread.Sleep(1000);

            // Try to connect again
            if (!_stop)
                ConnectAsync();
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Chat WebSocket client caught an error with code {error}");
        }

        private bool _stop;
    }
}
