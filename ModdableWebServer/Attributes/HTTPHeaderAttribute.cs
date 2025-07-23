namespace ModdableWebServer.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class HTTPHeaderAttribute(string method, string url, string headername, string headervalue = "", bool useContains = false) : HTTPAttribute(method, url)
{
    public string HeaderName = headername;
    public string HeaderValue = headervalue;
    public bool UseContains = useContains;
}
