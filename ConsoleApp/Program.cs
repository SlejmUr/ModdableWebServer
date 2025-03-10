﻿using ModdableWebServer.Attributes;
using ModdableWebServer.Helper;
using ModdableWebServer;
using ModdableWebServer.Servers;
using NetCoreServer;
using System.Net.Sockets;
using System.Reflection;

namespace ConsoleApp;

internal class Program
{

    [HTTP("GET", "/yeet")]
    public static bool Yeet(HttpRequest _, ServerStruct serverStruct)
    {
        serverStruct.Response.MakeGetResponse("yeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeet");
        serverStruct.SendResponse();
        return true;
    }

    [HTTP("GET", "/test")]
    public static bool Test3333(HttpRequest _, ServerStruct serverStruct)
    {
        serverStruct.Response.MakeGetResponse("teseststs");
        serverStruct.SendResponse();
        return true;
    }

    static void Main(string[] _)
    {
        DebugPrinter.EnableLogs = true;
        DebugPrinter.PrintToConsole = true;
        Console.WriteLine("Hello, World!");
        //CertHelper.GetContextNoValidate( SslProtocols.Tls12, "mypfx.pfx", "asecurepassword");
        var server = new WS_Server("127.0.0.1", 7777);
        //this override all attributes
        server.OverrideAttributes(Assembly.GetEntryAssembly()!);
        server.ReceivedRequestError += ReceivedRequestError;
        server.WSError += WSError;
        server.OnSocketError += OnSocketError;
        server.Started += HTTP_Server_OnStarted;
        server.Stopped += HTTP_Server_OnStopped;
        // In net8 this is a must be!
        server.AddStaticContent("static", string.Empty);
        server.Start();
        Console.ReadLine();
        server.Stop();
        Console.ReadLine();
    }

    private static void OnSocketError(object? sender, SocketError error)
    {
        Console.WriteLine("error " + error);
    }

    private static void WSError(object? sender, string wserror)
    {
        Console.WriteLine("error " + wserror);
    }

    private static void HTTP_Server_OnStopped(object? sender, EventArgs e)
    {
        Console.WriteLine("server stopped");
    }

    private static void HTTP_Server_OnStarted(object? sender, (string address, int port) e)
    {
        Console.WriteLine($"Server started: http://{e.address}:{e.port}");
    }

    private static void ReceivedRequestError(object? sender, (HttpRequest request, string error) sent)
    {
        Console.WriteLine($"HTTP_Server ReceivedRequestError: {sent.error}");
    }
}