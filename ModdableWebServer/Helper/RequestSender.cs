using NetCoreServer;
using System.Reflection;

namespace ModdableWebServer.Helper
{
    public class RequestSender
    {
        public static bool SendRequestHTTP(ServerStruct servers, HttpRequest request, Dictionary<(string url, string method), MethodInfo> AttributeToMethods)
        {
            Dictionary<string, string> Parameters = new();
            string url = request.Url;
            url = Uri.UnescapeDataString(url);
            bool Sent = false;
            foreach (var item in AttributeToMethods)
            {
                if ((UrlHelper.Match(url, item.Key.url, out Parameters) || item.Key.url == url) && request.Method.ToUpper() == item.Key.method.ToUpper())
                {
                    servers.Headers = request.GetHeaders();
                    servers.Parameters = Parameters;
                    Sent = (bool)item.Value.Invoke(servers, new object[] { request, servers })!;
                    break;
                }

            }
            return Sent;
        }

        public static void SendRequestWS(WebSocketStruct wsStruct, Dictionary<string, MethodInfo> wsMethods)
        {
            Dictionary<string, string> Parameters = new();
            foreach (var item in wsMethods)
            {
                if (UrlHelper.Match(wsStruct.Request.Url, item.Key, out Parameters) || item.Key == wsStruct.Request.Url)
                {
                    wsStruct.Request.Parameters = Parameters;
                    _ = item.Value.Invoke(wsStruct, new object[] { wsStruct })!;
                    break;
                }

            }
        }
    }
}
