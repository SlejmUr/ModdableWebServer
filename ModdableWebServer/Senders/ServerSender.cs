using ModdableWebServer.Attributes;
using ModdableWebServer.Helper;
using ModdableWebServer.Interfaces;
using System.Reflection;

namespace ModdableWebServer.Senders;

public class ServerSender
{
    public required ISession Session { get; init; }
    public required IServer Server { get; init; }
    public HttpRequest Request { get; internal set; } = default!;
    public Dictionary<string, string> Headers { get; } = [];
    public Dictionary<string, string> Parameters { get; internal set; } = [];
    public ServerType ServerType { get; init; }
    public HttpResponse Response => Session.Response;

    public virtual void SendResponse()
    {
        Session.SendResponse();
    }

    public virtual void SendResponse(HttpResponse response)
    {
        Session.SendResponse(response);
    }

    public virtual bool SendRequest(HttpRequest request)
    {
        Request = request;
        string url = Uri.UnescapeDataString(Request.Url);
        Log.Verbose("Requesting with URL: {url}", url);
        bool Sent = false;
        foreach (var item in Server.HTTPAttributeToMethods)
        {
            if ((item.Key.Url == url || UrlHelper.Match(url, item.Key.Url, out Dictionary<string, string> out_params)) &&
                Request.Method.Equals(item.Key.Method, StringComparison.CurrentCultureIgnoreCase))
            {
                Log.Verbose($"URL Matched! {url}");
                Request.PopulateHeaders(Headers);
                Parameters = out_params;
                Sent = InvokeMethod(item.Value);
                Log.Verbose("Invoked!");
            }
        }
        foreach (var item in Server.HeaderAttributeToMethods)
        {
            if ((item.Key.Url == url || UrlHelper.Match(url, item.Key.Url, out Dictionary<string, string> out_params)) &&
                Request.Method.Equals(item.Key.Method, StringComparison.CurrentCultureIgnoreCase))
            {
                Log.Verbose($"URL Matched! {url}");
                Request.PopulateHeaders(Headers);
                Parameters = out_params;
                Sent = SendHeader(item.Key, item.Value);
                Log.Verbose("Invoked!");
            }
        }
        return Sent;
    }

    internal bool InvokeMethod(MethodInfo method)
    {
        return (bool)method.Invoke(Server, [this])!;
    }

    private bool SendHeader(HTTPHeaderAttribute attribute, MethodInfo method)
    {
        if (string.IsNullOrEmpty(attribute.HeaderName))
            return false;
        if (!Headers.TryGetValue(attribute.HeaderName.ToLower(), out string? val))
            return false;
        if (string.IsNullOrEmpty(attribute.HeaderValue))
            return InvokeMethod(method);
        if (attribute.UseContains && val.Contains(attribute.HeaderValue))
            return InvokeMethod(method);
        if (val == attribute.HeaderValue)
            return InvokeMethod(method);
        return false;
    }
}
