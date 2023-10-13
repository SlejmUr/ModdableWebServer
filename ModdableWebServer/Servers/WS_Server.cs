using NetCoreServer;
using System.Net.Sockets;
using System.Reflection;
using ModdableWebServer.Helper;
using ModdableWebServer.Attributes;

namespace ModdableWebServer.Servers
{
    public class WS_Server : WsServer
    {
        #region Events
        public EventHandler<(HttpRequest request, string error)> ReceivedRequestError;
        public EventHandler<SocketError> OnSocketError;
        public EventHandler<string> WSError;
        public event EventHandler<(string address, int port)> Started;
        public event EventHandler Stopped;
        #endregion

        public Dictionary<(string url, string method), MethodInfo> HTTP_AttributeToMethods = new();
        public Dictionary<string, MethodInfo> WS_AttributeToMethods = new();

        public bool DoReturn404IfFail = true;
        public WS_Server(string address, int port) : base(address, port)
        {
            HTTP_AttributeToMethods = AttibuteMethodHelper.UrlHTTPLoader(Assembly.GetAssembly(typeof(HTTPAttribute)));
            WS_AttributeToMethods = AttibuteMethodHelper.UrlWSLoader(Assembly.GetAssembly(typeof(WSAttribute)));
        }
        protected override void OnStarted() => Started?.Invoke(this, (Address, Port));

        protected override void OnStopped() => Stopped?.Invoke(this, null);

        protected override TcpSession CreateSession() { return new Session(this); }

        protected override void OnError(SocketError error) => OnSocketError?.Invoke(this, error);

        public class Session : WsSession
        {
            public WS_Server WS_Server => (WS_Server)this.Server;
            public Session(WsServer server) : base(server) { }
            protected override void OnReceivedRequest(HttpRequest request)
            {
                Dictionary<string, string> Headers = new();
                for (int i = 0; i < request.Headers; i++)
                {
                    var headerpart = request.Header(i);
                    if (!Headers.ContainsKey(headerpart.Item1.ToLower()))
                        Headers.Add(headerpart.Item1.ToLower(), headerpart.Item2);
                }

                ServerStruct serverStruct = new ServerStruct()
                {
                    WS_Session = this,
                    Response = this.Response,
                    Enum = ServerEnum.WS
                };

                
                if (Headers.ContainsKey("websocket"))
                {
                    base.OnReceivedRequest(request);
                    return;
                }

                bool IsSent = RequestSender.SendRequestHTTP(serverStruct, request, WS_Server.HTTP_AttributeToMethods);

                if (WS_Server.DoReturn404IfFail && !IsSent)
                    SendResponse(Response.MakeErrorResponse(404));

            }

            WS_Struct ws_Struct;
            public override void OnWsConnected(HttpRequest request)
            {
                Console.WriteLine("OnWsConnected");
                Dictionary<string, string> Headers = new();
                for (int i = 0; i < request.Headers; i++)
                {
                    var headerpart = request.Header(i);
                    if (!Headers.ContainsKey(headerpart.Item1.ToLower()))
                        Headers.Add(headerpart.Item1.ToLower(), headerpart.Item2);
                }

                ws_Struct.IsConnected = true;
                ws_Struct.Request = new()
                { 
                    Body = request.Body,
                    Url = request.Url,
                    Headers = Headers
                };
                RequestSender.SendRequestWS(ws_Struct, WS_Server.WS_AttributeToMethods);
            }

            public override void OnWsReceived(byte[] buffer, long offset, long size)
            {
                Console.WriteLine("OnWsReceived");
                ws_Struct.WSRequest = new(buffer,offset,size);
                RequestSender.SendRequestWS(ws_Struct, WS_Server.WS_AttributeToMethods);
            }

            public override void OnWsDisconnected()
            {
                Console.WriteLine("OnWsDisconnected");
                ws_Struct.IsConnected = false;
                ws_Struct.WSRequest = null;
                RequestSender.SendRequestWS(ws_Struct, WS_Server.WS_AttributeToMethods);
            }

            public override void OnWsError(string error)
            {
                WS_Server.WSError?.Invoke(this,error);
            }

            protected override void OnReceivedRequestError(HttpRequest request, string error) => WS_Server.ReceivedRequestError?.Invoke(this, (request, error));

            protected override void OnError(SocketError error) => WS_Server.OnSocketError?.Invoke(this, error);
        }
    }
}
