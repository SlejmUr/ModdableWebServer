using ModdableWebServer.Attributes;
using System.Reflection;

namespace ModdableWebServer.Interfaces;

public interface IHttpServer : IServer
{
    bool DoReturn404IfFail { get; set; }
    public Dictionary<HTTPAttribute, MethodInfo> HTTPAttributeToMethods { get; }
    public Dictionary<HTTPHeaderAttribute, MethodInfo> HeaderAttributeToMethods { get; }

}
