using ModdableWebServer.Interfaces;
using ModdableWebServer.Senders;
using System.Net.Sockets;

namespace ModdableWebServer.Sessions;

public class HTTP_Session(HttpServer server) : HttpSession(server), ISession
{
    public IServer IServer => (IServer)Server;
    private ServerSender Sender = default!;

    protected override void OnConnecting()
    {
        Sender = new()
        {
            ServerType = ServerType.HTTP,
            Server = IServer,
            Session = this
        };
    }

    protected override void OnReceivedRequest(HttpRequest request)
    {
        if (request.Method == "GET" && !request.Url.Contains('?') && Cache.FindPath(request.Url))
        {
            var cache = Cache.Find(request.Url);
            // Check again to make sure.
            if (cache.Item1)
                SendAsync(cache.Item2);
        }

        bool isSent = Sender.SendRequest(request);
        if (!isSent)
            ServerEvents.OnReceivedFailed(IServer, request);

        if (IServer.DoReturn404IfFail && !isSent)
            SendResponse(Response.MakeErrorResponse(404));
    }

    protected override void OnReceivedRequestError(HttpRequest request, string error)
        => ServerEvents.OnReceivedRequestError(IServer, request, error);

    protected override void OnError(SocketError error)
        => ServerEvents.OnSocketError(IServer, error);
}
