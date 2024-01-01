using ModdableWebServer.Attributes;
using ModdableWebServer.Helper;
using NetCoreServer;
using System.Net.Sockets;
using System.Reflection;

namespace ModdableWebServer.Servers
{
    public class HTTP_Server : HttpServer
    {
        #region Events
        public EventHandler<(HttpRequest request, string error)>? ReceivedRequestError;
        public EventHandler<SocketError>? OnSocketError;
        public EventHandler<HttpRequest>? ReceivedFailed;
        public event EventHandler<(string address, int port)>? Started;
        public event EventHandler? Stopped;
        #endregion
        public Dictionary<(string url, string method), MethodInfo> AttributeToMethods = new();

        public bool DoReturn404IfFail = true;
        public HTTP_Server(string address, int port) : base(address, port)
        {
            AttributeToMethods = AttributeMethodHelper.UrlHTTPLoader(Assembly.GetAssembly(typeof(HTTPAttribute)));
        }

        #region Overrides
        protected override void OnStarted() => Started?.Invoke(this, (Address, Port));

        protected override void OnStopped() => Stopped?.Invoke(this, new());

        protected override TcpSession CreateSession() { return new Session(this); }

        protected override void OnError(SocketError error) => OnSocketError?.Invoke(this, error);
        #endregion

        public class Session : HttpSession
        {
            public Session(HttpServer server) : base(server) { }
            public HTTP_Server HTTP_Server => (HTTP_Server)this.Server;

            #region Overrides
            protected override void OnReceivedRequest(HttpRequest request)
            {
                ServerStruct serverStruct = new ServerStruct()
                {
                    HTTP_Session = this,
                    Response = this.Response,
                    Enum = ServerEnum.HTTP
                };

                bool IsSent = serverStruct.SendRequestHTTP(request, HTTP_Server.AttributeToMethods);
                DebugPrinter.Debug("[HttpSession.OnReceivedRequest] Request sent!");

                if (!IsSent)
                    HTTP_Server.ReceivedFailed?.Invoke(this, request);

                if (HTTP_Server.DoReturn404IfFail && !IsSent)
                    SendResponse(Response.MakeErrorResponse(404));
            }

            protected override void OnReceivedRequestError(HttpRequest request, string error) => HTTP_Server.ReceivedRequestError?.Invoke(this, (request, error));

            protected override void OnError(SocketError error) => HTTP_Server.OnSocketError?.Invoke(null, error);
            #endregion
        }
    }
}
