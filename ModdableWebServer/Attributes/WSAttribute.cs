namespace ModdableWebServer.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class WSAttribute(string url) : Attribute
{
    public string url = url;
}
