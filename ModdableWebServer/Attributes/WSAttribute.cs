namespace ModdableWebServer.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class WSAttribute(string url, WebSocketMethodListen listenMethod = WebSocketMethodListen.All) : Attribute
{
    public string Url = url;
    public WebSocketMethodListen ListenMethod = listenMethod;
}
