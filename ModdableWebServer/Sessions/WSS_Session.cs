using ModdableWebServer.Helper;
using ModdableWebServer.Interfaces;
using ModdableWebServer.Senders;
using System.Net.Sockets;

namespace ModdableWebServer.Sessions;

public class WSS_Session(WssServer server) : WssSession(server), IWSSession
{
    public IServer IServer => (IServer)Server;
    private WebSocketSender Sender = default!;
    #region Overrides
    protected override void OnConnecting()
    {
        Sender = new()
        {
            ServerType = ServerType.WSS,
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

        //  this could check
        //  if (request.GetHeaders()["upgrade"] == "websocket" && request.GetHeaders()["connection"] == "Upgrade")
        if (request.GetHeaders().ContainsValue("websocket"))
        {
            Log.Verbose("[OnReceivedRequest] Websocket Value on Headers Send back to base!");
            base.OnReceivedRequest(request);
            return;
        }

        bool isSent = Sender.SendRequest(request);
        if (!isSent)
            ServerEvents.OnReceivedFailed(IServer, request);

        if (IServer.DoReturn404IfFail && !isSent)
            SendResponse(Response.MakeErrorResponse(404));
    }

    public override void OnWsConnected(HttpRequest request)
    {
        Sender.Send(WebSocketMethodListen.Connected, request);
        Log.Verbose("Request sent!");
    }

    public override void OnWsConnecting(HttpRequest request)
    {
        Sender.Send(WebSocketMethodListen.Connecting, request);
        Log.Verbose("Request sent!");
    }


    public override void OnWsReceived(byte[] buffer, long offset, long size)
    {
        Sender.Buffer = buffer;
        Sender.Offset = offset;
        Sender.Size = size;
        Sender.Send(WebSocketMethodListen.Received, null);
        Log.Verbose("Request sent!");
    }

    public override void OnWsDisconnected()
    {
        Sender.Send(WebSocketMethodListen.Disconnected, null);
        Log.Verbose("Request sent!");
    }

    public override void OnWsDisconnecting()
    {
        Sender.Send(WebSocketMethodListen.Disconnecting, null);
        Log.Verbose("Request sent!");
    }

    public override void OnWsPing(byte[] buffer, long offset, long size)
    {
        Sender.Buffer = buffer;
        Sender.Offset = offset;
        Sender.Size = size;
        base.OnWsPing(buffer, offset, size);
        Sender.Send(WebSocketMethodListen.Ping, null);
        Log.Verbose("Request sent!");
    }

    public override void OnWsPong(byte[] buffer, long offset, long size)
    {
        Sender.Buffer = buffer;
        Sender.Offset = offset;
        Sender.Size = size;
        Sender.Send(WebSocketMethodListen.Pong, null);
        Log.Verbose("Request sent!");
    }

    public override void OnWsClose(byte[] buffer, long offset, long size, int status = 1000)
    {
        Sender.Buffer = buffer;
        Sender.Offset = offset;
        Sender.Size = size;
        Sender.CloseStatus = status;
        Sender.Send(WebSocketMethodListen.Close, null);
        Log.Verbose("Request sent!");
        base.OnWsClose(buffer, offset, size, status);
    }

    public override void OnWsError(string error)
        => ServerEvents.OnWebSocketError((IWSServer)IServer!, error);

    protected override void OnReceivedRequestError(HttpRequest request, string error)
    => ServerEvents.OnReceivedRequestError(IServer, request, error);

    protected override void OnError(SocketError error)
        => ServerEvents.OnSocketError(IServer, error);
    #endregion
}