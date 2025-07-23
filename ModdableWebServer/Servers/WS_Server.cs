using ModdableWebServer.Attributes;
using ModdableWebServer.Helper;
using ModdableWebServer.Interfaces;
using ModdableWebServer.Sessions;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace ModdableWebServer.Servers;

public class WS_Server : WsServer, IWSServer
{
    public bool DoReturn404IfFail { get; set; } = true;
    public Dictionary<WSAttribute, MethodInfo> WSAttributeToMethods { get; } = [];
    public Dictionary<HTTPAttribute, MethodInfo> HTTPAttributeToMethods { get; } = [];
    public Dictionary<HTTPHeaderAttribute, MethodInfo> HeaderAttributeToMethods { get; } = [];

    public WS_Server(string address, int port) : base(address, port) { }
    public WS_Server(IPAddress address, int port) : base(address, port) { }
    public WS_Server(DnsEndPoint endPoint) : base(endPoint) { }
    public WS_Server(IPEndPoint endPoint) : base(endPoint) { }

    #region Attribute Controls
    public void OverrideAttributes(Assembly assembly)
    {
        HeaderAttributeToMethods.Override(assembly);
        WSAttributeToMethods.Override(assembly);
        HTTPAttributeToMethods.Override(assembly);
    }

    public void MergeAttributes(Assembly assembly)
    {
        HeaderAttributeToMethods.Merge(assembly);
        WSAttributeToMethods.Merge(assembly);
        HTTPAttributeToMethods.Merge(assembly);
    }

    public void ClearAttributes()
    {
        HeaderAttributeToMethods.Clear();
        WSAttributeToMethods.Clear();
        HTTPAttributeToMethods.Clear();
    }

    #endregion

    #region Overrides
    protected override void OnStarted() => ServerEvents.OnStarted(this);
    protected override void OnStopped() => ServerEvents.OnStopped(this);
    protected override TcpSession CreateSession() => new WS_Session(this);
    protected override void OnError(SocketError error) => ServerEvents.OnSocketError(this, error);
    #endregion
}
