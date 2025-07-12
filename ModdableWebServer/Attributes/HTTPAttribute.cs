namespace ModdableWebServer.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class HTTPAttribute(string method, string url) : Attribute
{
    public string method = method;
    public string url = url;
}
