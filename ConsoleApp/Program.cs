using ModdableWebServer.Attributes;
using ModdableWebServer.Helper;
using ModdableWebServer;
using ModdableWebServer.Servers;
using NetCoreServer;
using System.Net.Sockets;

namespace ConsoleApp
{
    internal class Program
    {
        [HTTP("GET", "/yeet")]
        public static bool yeet(HttpRequest request, ServerStruct serverStruct)
        {
            serverStruct.Response.MakeGetResponse("yeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeet");
            ResponseSender.SendResponse(serverStruct);
            return true;
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            var server = new WS_Server("127.0.0.1",7777);
            server.ReceivedRequestError += ReceivedRequestError;
            server.WSError += WSError;
            server.OnSocketError += OnSocketError;
            server.Started += HTTP_Server_OnStarted;
            server.Stopped += HTTP_Server_OnStopped;
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
}