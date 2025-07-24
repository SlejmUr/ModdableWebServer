using ModdableWebServer.Helper;
using ModdableWebServer.Interfaces;

namespace ModdableWebServer.Senders;

public class WebSocketSender : ServerSender
{
    public byte[] Buffer { get; set; } = [];
    public long Offset { get; set; }
    public long Size { get; set; }
    public int CloseStatus { get; set; }

    private string? CachedURL;
    
    public WebSocketMethodListen CurrentMethod { get; protected set; }

    public void Send(WebSocketMethodListen currentMethod, HttpRequest? request = null)
    {
        if (currentMethod == WebSocketMethodListen.None)
            return;
        CurrentMethod = currentMethod;
        if (request != null)
            Request = request;
        if (!string.IsNullOrEmpty(Request.Url))
            CachedURL = Request.Url;
        if (string.IsNullOrEmpty(CachedURL))
            return;
        if (Server is not IWSServer webSocketServer)
            return;
        Dictionary<string, string> out_params = [];
        foreach (var item in webSocketServer.WSAttributeToMethods.Where(x => x.Key.ListenMethod.HasFlag(currentMethod)))
        {
            if (item.Key.Url == CachedURL || UrlHelper.Match(CachedURL, item.Key.Url, out out_params))
            {
                Log.Verbose($"URL Matched! {CachedURL}");
                Request.PopulateHeaders(Headers);
                // ONLY Connecting/Connected has headers, other stuff NOT!!
                Parameters = out_params;
                item.Value.Invoke(Server, [this]);
            }
        }
        Buffer = [];
        Offset = 0;
        Size = 0;
        CloseStatus = 0;
        CurrentMethod = WebSocketMethodListen.None;
    }
}