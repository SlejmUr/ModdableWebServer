using ModdableWebServer.Attributes;
using NetCoreServer;
using System.Reflection;

namespace ModdableWebServer.Helper;

public static class RequestSender
{
    public static bool SendRequestHTTP(this ServerStruct server, HttpRequest request, Dictionary<HTTPAttribute, MethodInfo> AttributeToMethods)
    {
        string url = Uri.UnescapeDataString(request.Url);
        DebugPrinter.Debug($"[SendRequestHTTP] Requesting with URL: {url}");
        bool Sent = false;
        foreach (var item in AttributeToMethods)
        {
            if ((UrlHelper.Match(url, item.Key.url, out Dictionary<string, string> Parameters) || item.Key.url == url) &&
                request.Method.ToLower() == item.Key.method.ToLower())
            {
                DebugPrinter.Debug($"[SendRequestHTTP] URL Matched! {url}");
                server.Headers = request.GetHeaders();
                server.Parameters = Parameters;
                Sent = (bool)item.Value.Invoke(server, [request, server])!;
                DebugPrinter.Debug("[SendRequestHTTP] Invoked!");
                break;
            }

        }
        return Sent;
    }

    public static bool SendRequestHTTPHeader(this ServerStruct server, HttpRequest request, Dictionary<HTTPHeaderAttribute, MethodInfo> AttributeToMethods)
    {
        string url = Uri.UnescapeDataString(request.Url);
        DebugPrinter.Debug($"[SendRequestHTTPHeader] Requesting with URL: {url}");
        bool Sent = false;
        foreach (var item in AttributeToMethods)
        {
            if ((UrlHelper.Match(url, item.Key.url, out Dictionary<string, string> Parameters) || item.Key.url == url) && 
                request.Method.Equals(item.Key.method, StringComparison.CurrentCultureIgnoreCase))
            {
                DebugPrinter.Debug($"[SendRequestHTTPHeader] URL Matched! {url}");
                server.Headers = request.GetHeaders();
                server.Parameters = Parameters;

                //sorry for much if states, testing
                if (!string.IsNullOrEmpty(item.Key.headername) && server.Headers.ContainsKey(item.Key.headername.ToLower()))
                {
                    DebugPrinter.Debug("[SendRequestHTTPHeader] Header contains the HeaderName! " + item.Key.headername);
                    if (item.Key.headervalue != string.Empty)
                    {
                        if (server.Headers[item.Key.headername] == item.Key.headervalue)
                        {
                            Sent = (bool)item.Value.Invoke(server, [request, server])!;
                            DebugPrinter.Debug("[SendRequestHTTPHeader] Invoked! (With Value)");
                            break;
                        }
                    }
                    else
                    {
                        Sent = (bool)item.Value.Invoke(server, [request, server])!;
                        DebugPrinter.Debug("[SendRequestHTTPHeader] Invoked! (Without Value)");
                        break;
                    }
                }
                //  Should we fallback or not? Maybe not for now
            }

        }
        return Sent;
    }

    public static void SendRequestWS(this WebSocketStruct wsStruct, Dictionary<string, MethodInfo> wsMethods)
    {
        foreach (var item in wsMethods)
        {
            if (UrlHelper.Match(wsStruct.Request.Url, item.Key, out Dictionary<string, string> Parameters) || item.Key == wsStruct.Request.Url)
            {
                DebugPrinter.Debug($"[SendRequestWS] URL Matched! {wsStruct.Request.Url}");
                wsStruct.Request.Parameters = Parameters;
                _ = item.Value.Invoke(wsStruct, [wsStruct])!;
                DebugPrinter.Debug("[SendRequestWS] Invoked!");
                break;
            }

        }
    }
}
