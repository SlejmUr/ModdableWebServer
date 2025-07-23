using ModdableWebServer.Attributes;
using System.Net;
using System.Reflection;

namespace ModdableWebServer.Interfaces;

public interface IServer
{
    bool DoReturn404IfFail { get; set; }

    Dictionary<HTTPAttribute, MethodInfo> HTTPAttributeToMethods { get; }
    Dictionary<HTTPHeaderAttribute, MethodInfo> HeaderAttributeToMethods { get; }

    Guid Id { get; }
    string Address { get; }
    int Port { get; }
    EndPoint Endpoint { get; }
    bool IsStarted { get; }

    bool Start();
    bool Stop();
    bool Restart();
    bool DisconnectAll();

    bool Multicast(ReadOnlySpan<byte> buffer);
    bool Multicast(ReadOnlySpan<char> text);

    bool IsDisposed { get; }
    bool IsSocketDisposed { get; }
}
