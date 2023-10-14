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
        public EventHandler<HttpRequest> ReceivedFailed;
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

        #region Overrides
        protected override void OnStarted() => Started?.Invoke(this, (Address, Port));

        protected override void OnStopped() => Stopped?.Invoke(this, new());

        protected override TcpSession CreateSession() { return new Session(this); }

        protected override void OnError(SocketError error) => OnSocketError?.Invoke(this, error);
        #endregion

        public class Session : WsSession
        {
            internal WebSocketStruct ws_Struct;
            public WS_Server WS_Server => (WS_Server)this.Server;
            public Session(WsServer server) : base(server) { }

            #region Overrides
            protected override void OnReceivedRequest(HttpRequest request)
            {
                ServerStruct serverStruct = new ServerStruct()
                {
                    WS_Session = this,
                    Response = this.Response,
                    Enum = ServerEnum.WS
                };
                
                if (request.GetHeaders().ContainsKey("websocket"))
                {
                    base.OnReceivedRequest(request);
                    return;
                }

                bool IsSent = RequestSender.SendRequestHTTP(serverStruct, request, WS_Server.HTTP_AttributeToMethods);

                if (!IsSent)
                    WS_Server.ReceivedFailed?.Invoke(this,request);

                if (WS_Server.DoReturn404IfFail && !IsSent)
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
                    Enum = WSEnum.WS,
                    WS_Session = this,
                    WSS_Session = null
                };
                RequestSender.SendRequestWS(ws_Struct, WS_Server.WS_AttributeToMethods);
            }

            public override void OnWsReceived(byte[] buffer, long offset, long size)
            {
                ws_Struct.WSRequest = new(buffer,offset,size);
                RequestSender.SendRequestWS(ws_Struct, WS_Server.WS_AttributeToMethods);
            }

            public override void OnWsDisconnected()
            {
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
            #endregion
        }
    }
}
