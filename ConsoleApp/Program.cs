using ModdableWebServer;
using ModdableWebServer.Attributes;
using ModdableWebServer.Helper;
using ModdableWebServer.Interfaces;
using ModdableWebServer.Senders;
using ModdableWebServer.Servers;
using NetCoreServer;
using Serilog;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Reflection;

namespace ConsoleApp;

internal class Program
{

    [HTTP("GET", "/yeet")]
    public static bool Yeet(ServerSender server)
    {
        server.Session.Response.MakeGetResponse("yeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeet");
        server.Session.SendResponse();
        return true;
    }

    [HTTP("GET", "/test")]
    public static bool Test3333(ServerSender server)
    {
        server.Session.Response.MakeGetResponse("teseststs");
        server.Session.SendResponse();
        return true;
    }

    static void Main(string[] _)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.File("logs.txt", outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
        Console.WriteLine("Hello, World!");
        //CertHelper.GetContextNoValidate( SslProtocols.Tls12, "mypfx.pfx", "asecurepassword");
        var server = new WS_Server("127.0.0.1", 7777);
        //this override all attributes
        server.OverrideAttributes(Assembly.GetAssembly(typeof(WS_Server))!);
        server.OverrideAttributes(Assembly.GetEntryAssembly()!);
        ServerEvents.ReceivedFailed += ServerEvents_ReceivedFailed;
        ServerEvents.ReceivedRequestError += ReceivedRequestError;
        ServerEvents.WebSocketError += WebSocketError_Event;
        ServerEvents.SocketError += OnSocketError;
        ServerEvents.Started += HTTP_Server_OnStarted;
        ServerEvents.Stopped += HTTP_Server_OnStopped;
        // In net8 this is a must be!
        server.AddStaticContent("static", string.Empty);
        server.Start();
        Console.ReadLine();
        server.Stop();
        Console.ReadLine();
    }

    private static void ServerEvents_ReceivedFailed(IServer arg1, HttpRequest arg2)
    {
        Console.WriteLine("ServerEvents_ReceivedFailed!");
    }

    private static void HTTP_Server_OnStopped(IServer server)
    {
        Console.WriteLine("HTTP_Server_OnStopped!");
    }

    private static void HTTP_Server_OnStarted(IServer server)
    {
        Console.WriteLine("HTTP_Server_OnStarted!");
    }

    private static void OnSocketError(IServer server, SocketError error)
    {
        Console.WriteLine("OnSocketError!");
    }

    private static void WebSocketError_Event(IWSServer server, string arg2)
    {
        Console.WriteLine("WebSocketError_Event!");
    }

    private static void ReceivedRequestError(IServer server, HttpRequest request, string arg3)
    {
        Console.WriteLine("ReceivedRequestError!");
    }
}