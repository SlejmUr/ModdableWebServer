using NetCoreServer;
using System.Net.Sockets;

namespace ModdableWebServer.Interfaces;

public interface ISession
{
    Guid Id { get; }
    IServer IServer { get; }
    Socket Socket { get; }
    bool IsConnected { get; }

    bool Disconnect();

    long Send(ReadOnlySpan<byte> buffer);

    long Send(string text);

    bool IsDisposed { get; }

    HttpResponse Response { get; }

    long SendResponse();

    long SendResponse(HttpResponse response);
}
