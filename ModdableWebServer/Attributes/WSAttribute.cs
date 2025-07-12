namespace ModdableWebServer.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class WSAttribute(string url) : Attribute
{
    public string url = url;
}
