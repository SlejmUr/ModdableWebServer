#if DEBUG
using ModdableWebServer.Attributes;
using ModdableWebServer.Helper;
using NetCoreServer;

namespace ModdableWebServer
{

    public class TEST
    {

        [WS("/ws/{test}")]
        public static void ws(WS_Struct ws_Struct)
        {
            Console.WriteLine("Headers:");
            foreach (var item in ws_Struct.Request.Headers)
            {
                Console.WriteLine(item.Key + " = " + item.Value);
            }
            Console.WriteLine("Parameters:");

            foreach (var item in ws_Struct.Request.Parameters)
            {
                Console.WriteLine(item.Key + " = " + item.Value);
            }
            Console.WriteLine(ws_Struct.IsConnected);
            Console.WriteLine(ws_Struct.Request.Url);
            Console.WriteLine(ws_Struct.WSRequest?.size);
        }



        [HTTP("GET", "/test")]
        public static bool TEST1(HttpRequest request, ServerStruct serverStruct)
        {
            Console.WriteLine("Headers:");
            foreach (var item in serverStruct.Headers)
            {
                Console.WriteLine(item.Key + " = " + item.Value);
            }
            Console.WriteLine("Parameters:");

            foreach (var item in serverStruct.Parameters)
            {
                Console.WriteLine(item.Key + " = " + item.Value);
            }

            Console.WriteLine("TEST");
            serverStruct.Response.MakeOkResponse();
            ResponseSender.SendResponse(serverStruct);
            return true;
        }

        [HTTP("GET", "/test2/{test}")]
        public static bool test2(HttpRequest request, ServerStruct serverStruct)
        {
            Console.WriteLine("Headers:");
            foreach (var item in serverStruct.Headers)
            {
                Console.WriteLine(item.Key + " = " + item.Value);
            }
            Console.WriteLine("Parameters:");

            foreach (var item in serverStruct.Parameters)
            {
                Console.WriteLine(item.Key + " = " + item.Value);
            }

            Console.WriteLine("test2");
            serverStruct.Response.MakeGetResponse("test2");
            ResponseSender.SendResponse(serverStruct);
            return true;
        }

    }

}
#endif
