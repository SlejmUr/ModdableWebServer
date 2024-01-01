using ModdableWebServer.Attributes;
using ModdableWebServer.Helper;
using NetCoreServer;
using System.Net.Sockets;
using System.Reflection;

namespace ModdableWebServer.Servers
{
    public class WSS_Server : WssServer
    {
        #region Events
        public EventHandler<(HttpRequest request, string error)>? ReceivedRequestError = null;
        public EventHandler<SocketError>? OnSocketError = null;
        public EventHandler<string>? WSError = null;
        public EventHandler<HttpRequest>? ReceivedFailed = null;
        public event EventHandler<(string address, int port)>? Started = null;
        public event EventHandler? Stopped = null;
        #endregion
        public Dictionary<(string url, string method), MethodInfo> HTTP_AttributeToMethods = new();
        public Dictionary<string, MethodInfo> WS_AttributeToMethods = new();

        public bool DoReturn404IfFail = true;
        public WSS_Server(SslContext context, string address, int port) : base(context, address, port)
        {
            HTTP_AttributeToMethods = AttributeMethodHelper.UrlHTTPLoader(Assembly.GetAssembly(typeof(HTTPAttribute)));
            WS_AttributeToMethods = AttributeMethodHelper.UrlWSLoader(Assembly.GetAssembly(typeof(WSAttribute)));
        }
        #region Overrides
        protected override void OnStarted() => Started?.Invoke(this, (Address, Port));

        protected override void OnStopped() => Stopped?.Invoke(this, new());

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

                //  this could check
                //  if (request.GetHeaders()["upgrade"] == "websocket" && request.GetHeaders()["connection"] == "Upgrade")
                if (request.GetHeaders().ContainsValue("websocket"))
                {
                    DebugPrinter.Debug("[WssSession.OnReceivedRequest] websocket Value on Headers Send back to base!");
                    base.OnReceivedRequest(request);
                    return;
                }

                bool IsSent = serverStruct.SendRequestHTTP(request, WSS_Server.HTTP_AttributeToMethods);
                DebugPrinter.Debug("[WssSession.OnReceivedRequest] Request sent!");

                if (!IsSent)
                    WSS_Server.ReceivedFailed?.Invoke(this, request);

                if (WSS_Server.DoReturn404IfFail && !IsSent)
                    SendResponse(Response.MakeErrorResponse(404));

            }

            public override void OnWsConnected(HttpRequest request)
            {
                ws_Struct = new()
                {
                    IsConnecting = false,
                    IsConnected = true,
                    IsClosed = false,
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
                DebugPrinter.Debug("[WssSession.OnWsConnected] Request sent!");
                ws_Struct.SendRequestWS(WSS_Server.WS_AttributeToMethods);
            }

            public override void OnWsConnecting(HttpRequest request)
            {
                ws_Struct = new()
                {
                    IsConnecting = true,
                    IsConnected = false,
                    IsClosed = false,
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
                DebugPrinter.Debug("[WssSession.OnWsConnecting] Request sent!");
                ws_Struct.SendRequestWS(WSS_Server.WS_AttributeToMethods);
            }

            public override bool OnWsConnecting(HttpRequest request, HttpResponse response)
            {
                ws_Struct = new()
                {
                    IsConnecting = true,
                    IsConnected = false,
                    IsClosed = false,
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
                DebugPrinter.Debug("[WssSession.OnWsConnecting] Request sent!");
                ws_Struct.SendRequestWS(WSS_Server.WS_AttributeToMethods);
                return base.OnWsConnecting(request, response);
            }


            public override void OnWsReceived(byte[] buffer, long offset, long size)
            {
                ws_Struct.WSRequest = new(buffer, offset, size);
                DebugPrinter.Debug("[WssSession.OnWsReceived] Request sent!");
                ws_Struct.SendRequestWS(WSS_Server.WS_AttributeToMethods);
            }

            public override void OnWsDisconnected()
            {
                ws_Struct.IsConnecting = false;
                ws_Struct.IsConnected = false;
                ws_Struct.IsClosed = true;
                ws_Struct.WSRequest = null;
                DebugPrinter.Debug("[WssSession.OnWsDisconnected] Request sent!");
                ws_Struct.SendRequestWS(WSS_Server.WS_AttributeToMethods);
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
