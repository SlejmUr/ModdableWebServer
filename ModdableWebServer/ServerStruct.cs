using ModdableWebServer.Servers;
using NetCoreServer;

namespace ModdableWebServer
{
    public struct ServerStruct
    {
        public HTTP_Server.Session? HTTP_Session;
        public HTTPS_Server? HTTPS_Session;
        public WS_Server.Session? WS_Session;
        public WSS_Server? WSS_Session;
        public HttpResponse Response;
        public Dictionary<string, string> Headers;
        public Dictionary<string, string> Parameters;
        public ServerEnum Enum;
    }

    public struct WS_Struct
    {
        public bool IsConnected;
        public Req Request;
        public (byte[] buffer, long offset, long size)? WSRequest;
    }

    public struct Req
    {
        public string Url;
        public Dictionary<string, string> Headers;
        public Dictionary<string, string> Parameters;
        public string Body;
    }

    public enum ServerEnum
    { 
        HTTP,
        HTTPS,
        WS,
        WSS
    };
}
