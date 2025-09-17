using System.Net.Sockets;

namespace ModdableWebServer.Interfaces;

public interface IHttpSession : ISession
{
    Socket Socket { get; }
    HttpResponse Response { get; }

    long SendResponse();

    long SendResponse(HttpResponse response);
}
