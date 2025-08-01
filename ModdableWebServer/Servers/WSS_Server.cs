﻿using ModdableWebServer.Attributes;
using ModdableWebServer.Helper;
using ModdableWebServer.Interfaces;
using ModdableWebServer.Sessions;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace ModdableWebServer.Servers;

public class WSS_Server : WssServer, IWSServer
{
    public bool DoReturn404IfFail { get; set; } = true;
    public Dictionary<WSAttribute, MethodInfo> WSAttributeToMethods { get; } = [];
    public Dictionary<HTTPAttribute, MethodInfo> HTTPAttributeToMethods { get; } = [];
    public Dictionary<HTTPHeaderAttribute, MethodInfo> HeaderAttributeToMethods { get; } = [];

    public WSS_Server(SslContext context, string address, int port) : base(context, address, port) { }
    public WSS_Server(SslContext context, IPAddress address, int port) : base(context, address, port) { }
    public WSS_Server(SslContext context, DnsEndPoint endPoint) : base(context, endPoint) { }
    public WSS_Server(SslContext context, IPEndPoint endPoint) : base(context, endPoint) { }

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

    protected override void OnStarted() => ServerEvents.OnStarted(this);
    protected override void OnStopped() => ServerEvents.OnStopped(this);
    protected override SslSession CreateSession() => new WSS_Session(this);
    protected override void OnError(SocketError error) => ServerEvents.OnSocketError(this, error);
}
