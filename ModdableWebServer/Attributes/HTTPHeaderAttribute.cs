namespace ModdableWebServer.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class HTTPHeaderAttribute : HTTPAttribute
{
    public string headername;
    public string headervalue;

    public HTTPHeaderAttribute(string method, string url, string headername) : base(method, url)
    {
        this.method = method;
        this.url = url;
        this.headername = headername;
        this.headervalue = string.Empty;
    }

    public HTTPHeaderAttribute(string method, string url, string headername, string headervalue) : base(method, url)
    {
        this.headername = headername;
        this.headervalue = headervalue;
    }
}
