using ModdableWebServer.Interfaces;
using ModdableWebServer.Senders;
using System.Net.Sockets;

namespace ModdableWebServer.Sessions;

public class HTTPS_Session(HttpsServer server) : HttpsSession(server), IHttpSession
{
    public IServer IServer => (IServer)Server;
    private ServerSender Sender = default!;

    protected override void OnConnecting()
    {
        Sender = new()
        {
            Server = (IHttpServer)Server,
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