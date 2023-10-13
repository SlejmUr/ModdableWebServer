using NetCoreServer;
using System.Net.Sockets;
using System.Reflection;
using ModdableWebServer.Helper;
using ModdableWebServer.Attributes;

namespace ModdableWebServer.Servers
{
    public class WSS_Server : WssServer
    {
        #region Events
        public EventHandler<(HttpRequest request, string error)> ReceivedRequestError;
        public EventHandler<SocketError> OnSocketError;
        public EventHandler<string> WSError;
        public EventHandler<HttpRequest> ReceivedFailed;
        public event EventHandler<(string address, int port)> Started;
        public event EventHandler Stopped;
        #endregion
        public Dictionary<(string url, string method), MethodInfo> HTTP_AttributeToMethods = new();
        public Dictionary<string, MethodInfo> WS_AttributeToMethods = new();

        public bool DoReturn404IfFail = true;
        public WSS_Server(SslContext context, string address, int port) : base(context, address, port)
        {
            HTTP_AttributeToMethods = AttibuteMethodHelper.UrlHTTPLoader(Assembly.GetAssembly(typeof(HTTPAttribute)));
            WS_AttributeToMethods = AttibuteMethodHelper.UrlWSLoader(Assembly.GetAssembly(typeof(WSAttribute)));
        }
        #region Overrides
        protected override void OnStarted() => Started?.Invoke(this, (Address, Port));

        protected override void OnStopped() => Stopped?.Invoke(this, null);

        protected override SslSession CreateSession() { return new Session(this); }

        protected override void OnError(SocketError error) => OnSocketError?.Invoke(this, error);
        #endregion

        public class Session : WssSession
        {
            internal WebSocketStruct ws_Struct;
            public WSS_Server WSS_Server => (WSS_Server)this.Server;
            public Session(WssServer server) : base(server) { }

            #region Overrides
            protected override void OnReceivedRequest(HttpRequest request)
            {
                ServerStruct serverStruct = new ServerStruct()
                {
                    WSS_Session = this,
                    Response = this.Response,
                    Enum = ServerEnum.WSS
                };

                if (request.GetHeaders().ContainsKey("websocket"))
                {
                    base.OnReceivedRequest(request);
                    return;
                }

                bool IsSent = RequestSender.SendRequestHTTP(serverStruct, request, WSS_Server.HTTP_AttributeToMethods);

                if (!IsSent)
                    WSS_Server.ReceivedFailed?.Invoke(this, request);

                if (WSS_Server.DoReturn404IfFail && !IsSent)
                    SendResponse(Response.MakeErrorResponse(404));

            }

            public override void OnWsConnected(HttpRequest request)
            {
                ws_Struct = new()
                {
                    IsConnected = true,
                    Request = new()
                    {
                        Body = request.Body,
                        Url = request.Url,
                        Headers = request.GetHeaders()
                    },
                    WSRequest = null,
                    Enum = WSEnum.WSS,
                    WS_Session = null,
                    WSS_Session = this
                };
                RequestSender.SendRequestWS(ws_Struct, WSS_Server.WS_AttributeToMethods);
            }

            public override void OnWsReceived(byte[] buffer, long offset, long size)
            {
                ws_Struct.WSRequest = new(buffer, offset, size);
                RequestSender.SendRequestWS(ws_Struct, WSS_Server.WS_AttributeToMethods);
            }

            public override void OnWsDisconnected()
            {
                ws_Struct.IsConnected = false;
                ws_Struct.WSRequest = null;
                RequestSender.SendRequestWS(ws_Struct, WSS_Server.WS_AttributeToMethods);
            }

            public override void OnWsError(string error)
            {
                WSS_Server.WSError?.Invoke(this, error);
            }

            protected override void OnReceivedRequestError(HttpRequest request, string error) => WSS_Server.ReceivedRequestError?.Invoke(this, (request, error));

            protected override void OnError(SocketError error) => WSS_Server.OnSocketError?.Invoke(this, error);
            #endregion

        }
    }
}
