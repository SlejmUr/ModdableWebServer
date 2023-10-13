using NetCoreServer;
using System.Net.Sockets;
using System.Reflection;
using ModdableWebServer.Helper;
using ModdableWebServer.Attributes;
using System.Reflection.PortableExecutable;

namespace ModdableWebServer.Servers
{
    public class HTTP_Server : HttpServer
    {
        #region Events
        public EventHandler<(HttpRequest request, string error)> ReceivedRequestError;
        public EventHandler<SocketError> OnSocketError;
        public event EventHandler<(string address, int port)> Started;
        public event EventHandler Stopped;
        #endregion

        public Dictionary<(string url, string method), MethodInfo> AttributeToMethods = new();

        public bool DoReturn404IfFail = true;
        public HTTP_Server(string address, int port) : base(address, port)
        {
            AttributeToMethods = AttibuteMethodHelper.UrlHTTPLoader(Assembly.GetAssembly(typeof(HTTPAttribute)));
        }

        protected override void OnStarted() => Started?.Invoke(this, (Address, Port));

        protected override void OnStopped() => Stopped?.Invoke(this, null);

        protected override TcpSession CreateSession() { return new Session(this); }

        protected override void OnError(SocketError error) => OnSocketError?.Invoke(this, error);

        public class Session : HttpSession
        {
            public HTTP_Server HTTP_Server => (HTTP_Server)this.Server;

            public Session(HttpServer server) : base(server) { }

            protected override void OnReceivedRequest(HttpRequest request)
            {
                ServerStruct serverStruct = new ServerStruct()
                { 
                    HTTP_Session = this,
                    Response = this.Response,
                    Enum = ServerEnum.HTTP
                }; 


                bool IsSent = RequestSender.SendRequestHTTP(serverStruct, request, HTTP_Server.AttributeToMethods);

                if (HTTP_Server.DoReturn404IfFail && !IsSent)
                    SendResponse(Response.MakeErrorResponse(404));

            }

            protected override void OnReceivedRequestError(HttpRequest request, string error) => HTTP_Server.ReceivedRequestError?.Invoke(this, (request, error));

            protected override void OnError(SocketError error) => HTTP_Server.OnSocketError?.Invoke(null, error);
        }
    }
}
