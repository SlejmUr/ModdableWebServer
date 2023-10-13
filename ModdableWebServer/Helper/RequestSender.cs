using NetCoreServer;
using System.Reflection;

namespace ModdableWebServer.Helper
{
    public class RequestSender
    {
        public static bool SendRequestHTTP(ServerStruct servers, HttpRequest request, Dictionary<(string url, string method), MethodInfo> AttributeToMethods)
        {
            Dictionary<string, string> Headers = new();
            Dictionary<string, string> Parameters = new();
            Headers.Clear();
            for (int i = 0; i < request.Headers; i++)
            {
                var headerpart = request.Header(i);
                if (!Headers.ContainsKey(headerpart.Item1.ToLower()))
                    Headers.Add(headerpart.Item1.ToLower(), headerpart.Item2);
            }
            string url = request.Url;
            url = Uri.UnescapeDataString(url);
            bool Sent = false;
            foreach (var item in AttributeToMethods)
            {
                if ((UrlHelper.Match(url, item.Key.url, out Parameters) || item.Key.url == url) && request.Method.ToUpper() == item.Key.method.ToUpper())
                {
                    servers.Headers = Headers;
                    servers.Parameters = Parameters;
                    Sent = (bool)item.Value.Invoke(servers, new object[] { request, servers })!;
                    break;
                }

            }
            return Sent;
        }

        public static void SendRequestWS(WS_Struct wsStruct, Dictionary<string, MethodInfo> wsMethods)
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
