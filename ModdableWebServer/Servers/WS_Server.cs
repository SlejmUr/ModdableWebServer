﻿using ModdableWebServer.Attributes;
using ModdableWebServer.Helper;
using NetCoreServer;
using System.Net.Sockets;
using System.Reflection;

namespace ModdableWebServer.Servers
{
    public class WS_Server : WsServer
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
        public WS_Server(string address, int port) : base(address, port)
        {
            HTTP_AttributeToMethods = AttributeMethodHelper.UrlHTTPLoader(Assembly.GetAssembly(typeof(HTTPAttribute)));
            WS_AttributeToMethods = AttributeMethodHelper.UrlWSLoader(Assembly.GetAssembly(typeof(WSAttribute)));
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

                if (request.GetHeaders().ContainsValue("websocket"))
                {
                    DebugPrinter.Debug("[WsSession.OnReceivedRequest] websocket Value on Headers Send back to base!");
                    base.OnReceivedRequest(request);
                    return;
                }

                bool IsSent = RequestSender.SendRequestHTTP(serverStruct, request, WS_Server.HTTP_AttributeToMethods);
                DebugPrinter.Debug("[WsSession.OnReceivedRequest] Request sent!");

                if (!IsSent)
                    WS_Server.ReceivedFailed?.Invoke(this, request);

                if (WS_Server.DoReturn404IfFail && !IsSent)
                    SendResponse(Response.MakeErrorResponse(404));

            }

            public override void OnWsConnected(HttpRequest request)
            {
                ws_Struct = new()
                {
                    IsConnecting = false,
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
                DebugPrinter.Debug("[WsSession.OnWsConnected] Request sent!");
                RequestSender.SendRequestWS(ws_Struct, WS_Server.WS_AttributeToMethods);
            }

            public override void OnWsConnecting(HttpRequest request)
            {
                ws_Struct = new()
                {
                    IsConnecting = true,
                    IsConnected = false,
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
                DebugPrinter.Debug("[WsSession.OnWsConnecting] Request sent!");
                RequestSender.SendRequestWS(ws_Struct, WS_Server.WS_AttributeToMethods);
            }

            public override void OnWsReceived(byte[] buffer, long offset, long size)
            {
                ws_Struct.WSRequest = new(buffer, offset, size);
                DebugPrinter.Debug("[WsSession.OnWsReceived] Request sent!");
                RequestSender.SendRequestWS(ws_Struct, WS_Server.WS_AttributeToMethods);
            }

            public override void OnWsDisconnected()
            {
                ws_Struct.IsConnected = false;
                ws_Struct.WSRequest = null;
                DebugPrinter.Debug("[WsSession.OnWsDisconnected] Request sent!");
                RequestSender.SendRequestWS(ws_Struct, WS_Server.WS_AttributeToMethods);
            }

            public override void OnWsError(string error)
            {
                WS_Server.WSError?.Invoke(this, error);
            }

            protected override void OnReceivedRequestError(HttpRequest request, string error) => WS_Server.ReceivedRequestError?.Invoke(this, (request, error));

            protected override void OnError(SocketError error) => WS_Server.OnSocketError?.Invoke(this, error);
            #endregion
        }
    }
}
