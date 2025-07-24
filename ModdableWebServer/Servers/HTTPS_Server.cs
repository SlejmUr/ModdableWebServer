using ModdableWebServer.Attributes;
using ModdableWebServer.Helper;
using ModdableWebServer.Interfaces;
using ModdableWebServer.Sessions;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace ModdableWebServer.Servers;

public class HTTPS_Server : HttpsServer, IServer
{
    public bool DoReturn404IfFail { get; set; } = true;
    public Dictionary<HTTPAttribute, MethodInfo> HTTPAttributeToMethods { get; } = [];
    public Dictionary<HTTPHeaderAttribute, MethodInfo> HeaderAttributeToMethods { get; }= [];

    public HTTPS_Server(SslContext context, string address, int port) : base(context, address, port) { }
    public HTTPS_Server(SslContext context, IPAddress address, int port) : base(context, address, port) { }
    public HTTPS_Server(SslContext context, DnsEndPoint endpoint) : base(context, endpoint) { }
    public HTTPS_Server(SslContext context, IPEndPoint ipEndPoint) : base(context, ipEndPoint) { }

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

    protected override void OnStarted() => ServerEvents.OnStarted(this);
    protected override void OnStopped() => ServerEvents.OnStopped(this);
    protected override SslSession CreateSession() => new HTTPS_Session(this);
    protected override void OnError(SocketError error) => ServerEvents.OnSocketError(this, error);
}
