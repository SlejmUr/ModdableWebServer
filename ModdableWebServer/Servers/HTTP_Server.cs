using ModdableWebServer.Attributes;
using ModdableWebServer.Helper;
using NetCoreServer;
using System.Net;
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
        public Dictionary<HTTPAttribute, MethodInfo> AttributeToMethods = new();
        public Dictionary<HTTPHeaderAttribute, MethodInfo> HeaderAttributeToMethods = new();

        public bool DoReturn404IfFail = true;
        public HTTP_Server(string address, int port) : base(address, port)
        {
            AttributeToMethods = AttributeMethodHelper.UrlHTTPLoader(Assembly.GetAssembly(typeof(HTTPAttribute)));
            HeaderAttributeToMethods = AttributeMethodHelper.UrlHTTPHeaderLoader(Assembly.GetAssembly(typeof(HTTPHeaderAttribute)));
        }

        public HTTP_Server(IPAddress address, int port) : base(address, port)
        {
            AttributeToMethods = AttributeMethodHelper.UrlHTTPLoader(Assembly.GetAssembly(typeof(HTTPAttribute)));
            HeaderAttributeToMethods = AttributeMethodHelper.UrlHTTPHeaderLoader(Assembly.GetAssembly(typeof(HTTPHeaderAttribute)));
        }

        public HTTP_Server(DnsEndPoint endpoint) : base(endpoint)
        {
            AttributeToMethods = AttributeMethodHelper.UrlHTTPLoader(Assembly.GetAssembly(typeof(HTTPAttribute)));
            HeaderAttributeToMethods = AttributeMethodHelper.UrlHTTPHeaderLoader(Assembly.GetAssembly(typeof(HTTPHeaderAttribute)));
        }

        public HTTP_Server(IPEndPoint endPoint) : base(endPoint)
        {
            AttributeToMethods = AttributeMethodHelper.UrlHTTPLoader(Assembly.GetAssembly(typeof(HTTPAttribute)));
            HeaderAttributeToMethods = AttributeMethodHelper.UrlHTTPHeaderLoader(Assembly.GetAssembly(typeof(HTTPHeaderAttribute)));
        }

        #region Attribute Controls
        public void OverrideAttribute(Assembly assembly)
        {
            AttributeToMethods.Override(assembly);
        }

        public void MergeAttribute(Assembly assembly)
        {
            AttributeToMethods.Merge(assembly);
        }

        public void ClearAttribute()
        {
            AttributeToMethods.Clear();
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
            AttributeToMethods.Override(assembly);
        }

        public void MergeAttributes(Assembly assembly)
        {
            HeaderAttributeToMethods.Merge(assembly);
            AttributeToMethods.Merge(assembly);
        }

        public void ClearAttributes()
        {
            HeaderAttributeToMethods.Clear();
            AttributeToMethods.Clear();
        }

        #endregion
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
                if (request.Method == "GET" && !request.Url.Contains("?") && this.Cache.FindPath(request.Url))
                {
                    var cache = this.Cache.Find(request.Url);
                    // Check again to make sure.
                    if (cache.Item1)
                        this.SendAsync(cache.Item2);
                }

                ServerStruct serverStruct = new ServerStruct()
                {
                    HTTP_Session = this,
                    Response = this.Response,
                    Enum = ServerEnum.HTTP
                };

                bool IsSent = serverStruct.SendRequestHTTP(request, HTTP_Server.AttributeToMethods);
                bool IsSent_header = serverStruct.SendRequestHTTPHeader(request, HTTP_Server.HeaderAttributeToMethods);

                DebugPrinter.Debug("[HttpSession.OnReceivedRequest] Request sent!");

                if (!IsSent && !IsSent_header)
                    HTTP_Server.ReceivedFailed?.Invoke(this, request);

                if (HTTP_Server.DoReturn404IfFail && (!IsSent && !IsSent_header))
                    SendResponse(Response.MakeErrorResponse(404));
            }

            protected override void OnReceivedRequestError(HttpRequest request, string error) => HTTP_Server.ReceivedRequestError?.Invoke(this, (request, error));

            protected override void OnError(SocketError error) => HTTP_Server.OnSocketError?.Invoke(null, error);
            #endregion
        }
    }
}
