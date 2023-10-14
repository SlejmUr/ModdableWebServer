﻿using NetCoreServer;
using System.Net.Sockets;
using System.Reflection;
using ModdableWebServer.Helper;
using ModdableWebServer.Attributes;
using System.Net;

namespace ModdableWebServer.Servers
{
    public class HTTPS_Server : HttpsServer
    {
        #region Events
        public EventHandler<(HttpRequest request, string error)> ReceivedRequestError;
        public EventHandler<SocketError> OnSocketError;
        public EventHandler<HttpRequest> ReceivedFailed;
        public event EventHandler<(string address, int port)> Started;
        public event EventHandler Stopped;
        #endregion
        public Dictionary<(string url, string method), MethodInfo> AttributeToMethods = new();

        public bool DoReturn404IfFail = true;
        public HTTPS_Server(SslContext context, IPAddress address, int port) : base(context, address, port) 
        {
            AttributeToMethods = AttibuteMethodHelper.UrlHTTPLoader(Assembly.GetAssembly(typeof(HTTPAttribute)));
        }

        #region Overrides
        protected override void OnStarted() => Started?.Invoke(this, (Address, Port));

        protected override void OnStopped() => Stopped?.Invoke(this, new());

        protected override SslSession CreateSession() { return new Session(this); }

        protected override void OnError(SocketError error) => OnSocketError?.Invoke(this, error);
        #endregion

        public class Session : HttpsSession
        {
            public HTTPS_Server HTTPS_Server => (HTTPS_Server)this.Server;
            public Session(HttpsServer server) : base(server) { }

            #region Overrides
            protected override void OnReceivedRequest(HttpRequest request)
            {
                ServerStruct serverStruct = new ServerStruct()
                {
                    HTTPS_Session = this,
                    Response = this.Response,
                    Enum = ServerEnum.HTTPS
                };

                bool IsSent = RequestSender.SendRequestHTTP(serverStruct, request, HTTPS_Server.AttributeToMethods);

                if (!IsSent)
                    HTTPS_Server.ReceivedFailed?.Invoke(this, request);

                if (HTTPS_Server.DoReturn404IfFail && !IsSent)
                    SendResponse(Response.MakeErrorResponse(404));
            }

            protected override void OnReceivedRequestError(HttpRequest request, string error) => HTTPS_Server.ReceivedRequestError?.Invoke(this, (request, error));

            protected override void OnError(SocketError error) => HTTPS_Server.OnSocketError?.Invoke(null, error);
            #endregion
        }
    }
}
