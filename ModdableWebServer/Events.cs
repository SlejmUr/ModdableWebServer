using ModdableWebServer.Interfaces;
using System.Net.Sockets;

namespace ModdableWebServer;

public static class ServerEvents
{
    public static event Action<IServer, HttpRequest, string>? ReceivedRequestError;
    public static event Action<IServer, SocketError>? SocketError;
    public static event Action<IServer, HttpRequest>? ReceivedFailed;
    public static event Action<IWSServer, string>? WebSocketError;
    public static event Action<IServer>? Started;
    public static event Action<IServer>? Stopped;

    internal static void OnStopped(in IServer server)
        => Stopped?.Invoke(server);

    internal static void OnStarted(in IServer server)
        => Started?.Invoke(server);

    internal static void OnReceivedRequestError(in IServer server, in HttpRequest request, in string error)
        => ReceivedRequestError?.Invoke(server, request, error);

    internal static void OnSocketError(in IServer server, in SocketError error)
        => SocketError?.Invoke(server, error);

    internal static void OnReceivedFailed(in IServer server, in HttpRequest request)
        => ReceivedFailed?.Invoke(server, request);

    internal static void OnWebSocketError(in IWSServer server, in string error)
        => WebSocketError?.Invoke(server, error);
}
