namespace ModdableWebServer.Interfaces;

public interface IHttpSession : ISession
{
    HttpResponse Response { get; }

    long SendResponse();

    long SendResponse(HttpResponse response);
}
