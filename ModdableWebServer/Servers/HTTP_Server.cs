using ModdableWebServer.Attributes;
using ModdableWebServer.Helper;
using ModdableWebServer.Interfaces;
using ModdableWebServer.Sessions;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace ModdableWebServer.Servers;

public class HTTP_Server : HttpServer, IServer
{
    public bool DoReturn404IfFail { get; set; } = true;
    public Dictionary<HTTPAttribute, MethodInfo> HTTPAttributeToMethods { get; } = [];
    public Dictionary<HTTPHeaderAttribute, MethodInfo> HeaderAttributeToMethods { get; } = [];

    public HTTP_Server(string address, int port) : base(address, port) { }
    public HTTP_Server(IPAddress address, int port) : base(address, port) { }
    public HTTP_Server(DnsEndPoint endpoint) : base(endpoint) { }
    public HTTP_Server(IPEndPoint endPoint) : base(endPoint) { }
    
    #region Attribute Controls

    public void OverrideAttributes(Assembly assembly)
    {
        HeaderAttributeToMethods.Override(assembly);
        HTTPAttributeToMethods.Override(assembly);
    }

    public void MergeAttributes(Assembly assembly)
    {
        HeaderAttributeToMethods.Merge(assembly);
        HTTPAttributeToMethods.Merge(assembly);
    }

    public void ClearAttributes()
    {
        HeaderAttributeToMethods.Clear();
        HTTPAttributeToMethods.Clear();
    }
    #endregion
    protected override void OnStarted() => ServerEvents.OnStarted(this);
    protected override void OnStopped() => ServerEvents.OnStopped(this);
    protected override TcpSession CreateSession() => new HTTP_Session(this);
    protected override void OnError(SocketError error) => ServerEvents.OnSocketError(this, error);
}
