namespace ModdableWebServer.Helper;

public static class RequestExport
{
    public static Dictionary<string, string> GetHeaders(this HttpRequest request)
    {
        Dictionary<string, string> Headers = [];
        for (int i = 0; i < request.Headers; i++)
        {
            var headerpart = request.Header(i);
            if (!Headers.ContainsKey(headerpart.Item1.ToLower()))
                Headers.Add(headerpart.Item1.ToLower(), headerpart.Item2);
        }
        return Headers;
    }

    public static void PopulateHeaders(this HttpRequest request, Dictionary<string, string> Headers)
    {
        Headers.Clear();
        for (int i = 0; i < request.Headers; i++)
        {
            var headerpart = request.Header(i);
            if (!Headers.ContainsKey(headerpart.Item1.ToLower()))
                Headers.Add(headerpart.Item1.ToLower(), headerpart.Item2);
        }
    }
}
