namespace ModdableWebServer.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class HTTPHeaderAttribute : Attribute
{
    public string method;
    public string url;
    public string headername;
    public string headervalue;

    public HTTPHeaderAttribute(string method, string url, string headername)
    {
        this.method = method;
        this.url = url;
        this.headername = headername;
        this.headervalue = string.Empty;
    }

    public HTTPHeaderAttribute(string method, string url, string headername, string headervalue)
    {
        this.method = method;
        this.url = url;
        this.headername = headername;
        this.headervalue = headervalue;
    }
}
