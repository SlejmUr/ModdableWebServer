using NetCoreServer;

namespace ModdableWebServer;

public struct ServerStruct
{
    public TcpSession? HTTP_Session;
    public SslSession? HTTPS_Session;
    public TcpSession? WS_Session;
    public SslSession? WSS_Session;
    public HttpResponse Response;
    public Dictionary<string, string> Headers;
    public Dictionary<string, string> Parameters;
    public ServerEnum Enum;
}

public struct WebSocketStruct
{
    public bool IsConnecting;
    public bool IsConnected;
    public bool IsClosed;
    public Req Request;
    public (byte[] buffer, long offset, long size)? WSRequest;
    public TcpSession? WS_Session;
    public SslSession? WSS_Session;
    public WSEnum Enum;
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

public enum WSEnum
{
    WS,
    WSS
};
