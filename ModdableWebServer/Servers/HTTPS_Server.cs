using ModdableWebServer.Attributes;
using ModdableWebServer.Helper;
using NetCoreServer;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace ModdableWebServer.Servers
{
    public class HTTPS_Server : HttpsServer
    {
        #region Events
        public EventHandler<(HttpRequest request, string error)>? ReceivedRequestError = null;
        public EventHandler<SocketError>? OnSocketError = null;
        public EventHandler<HttpRequest>? ReceivedFailed = null;
        public event EventHandler<(string address, int port)>? Started = null;
        public event EventHandler? Stopped = null;
        #endregion
        public Dictionary<HTTPAttribute, MethodInfo> AttributeToMethods = new();
        public Dictionary<HTTPHeaderAttribute, MethodInfo> HeaderAttributeToMethods = new();

        public bool DoReturn404IfFail = true;
        public HTTPS_Server(SslContext context, IPAddress address, int port) : base(context, address, port)
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

                bool IsSent = serverStruct.SendRequestHTTP(request, HTTPS_Server.AttributeToMethods);
                bool IsSent_header = serverStruct.SendRequestHTTPHeader(request, HTTPS_Server.HeaderAttributeToMethods);

                DebugPrinter.Debug("[HttpsSession.OnReceivedRequest] Request sent!");

                if (!IsSent && !IsSent_header)
                    HTTPS_Server.ReceivedFailed?.Invoke(this, request);

                if (HTTPS_Server.DoReturn404IfFail && (!IsSent && !IsSent_header))
                    SendResponse(Response.MakeErrorResponse(404));
            }

            protected override void OnReceivedRequestError(HttpRequest request, string error) => HTTPS_Server.ReceivedRequestError?.Invoke(this, (request, error));

            protected override void OnError(SocketError error) => HTTPS_Server.OnSocketError?.Invoke(null, error);
            #endregion
        }
    }
}
