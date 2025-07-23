namespace ModdableWebServer.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class HTTPAttribute(string method, string url) : Attribute
{
    public string Method = method;
    public string Url = url;
}
