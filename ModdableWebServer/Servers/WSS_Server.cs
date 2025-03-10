﻿using ModdableWebServer.Attributes;
using ModdableWebServer.Helper;
using NetCoreServer;
using System.Net;
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
        public Dictionary<HTTPHeaderAttribute, MethodInfo> HeaderAttributeToMethods = [];
        public Dictionary<HTTPAttribute, MethodInfo> HTTP_AttributeToMethods = [];
        public Dictionary<string, MethodInfo> WS_AttributeToMethods = [];

        public bool DoReturn404IfFail = true;
        public WSS_Server(SslContext context, string address, int port) : base(context, address, port)
        {
            HTTP_AttributeToMethods = AttributeMethodHelper.UrlHTTPLoader(Assembly.GetAssembly(typeof(HTTPAttribute)));
            WS_AttributeToMethods = AttributeMethodHelper.UrlWSLoader(Assembly.GetAssembly(typeof(WSAttribute)));
            HeaderAttributeToMethods = AttributeMethodHelper.UrlHTTPHeaderLoader(Assembly.GetAssembly(typeof(HTTPHeaderAttribute)));
        }

        public WSS_Server(SslContext context, IPAddress address, int port) : base(context, address, port)
        {
            HTTP_AttributeToMethods = AttributeMethodHelper.UrlHTTPLoader(Assembly.GetAssembly(typeof(HTTPAttribute)));
            WS_AttributeToMethods = AttributeMethodHelper.UrlWSLoader(Assembly.GetAssembly(typeof(WSAttribute)));
            HeaderAttributeToMethods = AttributeMethodHelper.UrlHTTPHeaderLoader(Assembly.GetAssembly(typeof(HTTPHeaderAttribute)));
        }

        public WSS_Server(SslContext context, DnsEndPoint endPoint) : base(context, endPoint)
        {
            HTTP_AttributeToMethods = AttributeMethodHelper.UrlHTTPLoader(Assembly.GetAssembly(typeof(HTTPAttribute)));
            WS_AttributeToMethods = AttributeMethodHelper.UrlWSLoader(Assembly.GetAssembly(typeof(WSAttribute)));
            HeaderAttributeToMethods = AttributeMethodHelper.UrlHTTPHeaderLoader(Assembly.GetAssembly(typeof(HTTPHeaderAttribute)));
        }

        public WSS_Server(SslContext context, IPEndPoint endPoint) : base(context, endPoint)
        {
            HTTP_AttributeToMethods = AttributeMethodHelper.UrlHTTPLoader(Assembly.GetAssembly(typeof(HTTPAttribute)));
            WS_AttributeToMethods = AttributeMethodHelper.UrlWSLoader(Assembly.GetAssembly(typeof(WSAttribute)));
            HeaderAttributeToMethods = AttributeMethodHelper.UrlHTTPHeaderLoader(Assembly.GetAssembly(typeof(HTTPHeaderAttribute)));
        }

        #region Attribute Controls
        public void OverrideAttribute(Assembly assembly)
        {
            HTTP_AttributeToMethods.Override(assembly);
        }

        public void MergeAttribute(Assembly assembly)
        {
            HTTP_AttributeToMethods.Merge(assembly);
        }

        public void ClearAttribute()
        {
            HTTP_AttributeToMethods.Clear();
        }

        public void OverrideWSAttribute(Assembly assembly)
        {
            WS_AttributeToMethods.Override(assembly);
        }

        public void MergeWSAttribute(Assembly assembly)
        {
            WS_AttributeToMethods.Merge(assembly);
        }

        public void ClearWSAttribute()
        {
            WS_AttributeToMethods.Clear();
        }

        public void OverrideHeaderAttribute(Assembly assembly)
        {
            HeaderAttributeToMethods.Override(assembly);
        }

        public void MergeHeaderAttribute(Assembly assembly)
        {
            HeaderAttributeToMethods.Merge(assembly);
        }

        public void ClearHeaderAttribute()
        {
            HeaderAttributeToMethods.Clear();
        }

        public void OverrideAttributes(Assembly assembly)
        {
            HeaderAttributeToMethods.Override(assembly);
            WS_AttributeToMethods.Override(assembly);
            HTTP_AttributeToMethods.Override(assembly);
        }

        public void MergeAttributes(Assembly assembly)
        {
            HeaderAttributeToMethods.Merge(assembly);
            WS_AttributeToMethods.Merge(assembly);
            HTTP_AttributeToMethods.Merge(assembly);
        }

        public void ClearAttributes()
        {
            HeaderAttributeToMethods.Clear();
            WS_AttributeToMethods.Clear();
            HTTP_AttributeToMethods.Clear();
        }

        #endregion

        #region Overrides
        protected override void OnStarted() => Started?.Invoke(this, (Address, Port));

        protected override void OnStopped() => Stopped?.Invoke(this, new());

        protected override SslSession CreateSession() { return new Session(this); }

        protected override void OnError(SocketError error) => OnSocketError?.Invoke(this, error);
        #endregion

        public class Session(WssServer server) : WssSession(server)
        {
            internal WebSocketStruct ws_Struct;
            public WSS_Server WSS_Server => (WSS_Server)this.Server;

            #region Overrides
            protected override void OnReceivedRequest(HttpRequest request)
            {
                if (request.Method == "GET" && !request.Url.Contains('?') && this.Cache.FindPath(request.Url))
                {
                    var cache = this.Cache.Find(request.Url);
                    // Check again to make sure.
                    if (cache.Item1)
                        this.SendAsync(cache.Item2);
                }

                ServerStruct serverStruct = new()
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
                bool IsSent_header = serverStruct.SendRequestHTTPHeader(request, WSS_Server.HeaderAttributeToMethods);

                DebugPrinter.Debug($"[WssSession.OnReceivedRequest] Request sent! Normal? {IsSent} Header? {IsSent_header} ");

                if (!IsSent && !IsSent_header)
                    WSS_Server.ReceivedFailed?.Invoke(this, request);

                if (WSS_Server.DoReturn404IfFail && (!IsSent && !IsSent_header))
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
