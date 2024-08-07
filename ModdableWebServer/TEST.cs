﻿#if DEBUG
using ModdableWebServer.Attributes;
using ModdableWebServer.Helper;
using NetCoreServer;

namespace ModdableWebServer
{
    public class TEST
    {

        [WS("/ws/{test}")]
        public static void ws(WebSocketStruct ws_Struct)
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
            Console.WriteLine("IsConnected: " + ws_Struct.IsConnected);
            Console.WriteLine("IsConnecting: " + ws_Struct.IsConnecting);
            Console.WriteLine("IsClosed: " + ws_Struct.IsClosed);
            Console.WriteLine(ws_Struct.Request.Url);
            Console.WriteLine(ws_Struct.WSRequest?.buffer);
            Console.WriteLine(ws_Struct.WSRequest?.offset);
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

        [HTTPHeader("GET", "/testheader", "test")]
        public static bool TEST_HEADER(HttpRequest request, ServerStruct serverStruct)
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

            Console.WriteLine("TEST HEADER");
            ResponseCreator response = new();
            response.SetBody("test header");
            serverStruct.Response = response.GetResponse();
            ResponseSender.SendResponse(serverStruct);
            return true;
        }

        // IF we would use the same url we would have been down, since the other one is triggered before this one.
        [HTTPHeader("GET", "/testheader2", "test","myvalue")]
        public static bool TEST_HEADER2(HttpRequest request, ServerStruct serverStruct)
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

            Console.WriteLine("TEST HEADER 2");
            ResponseCreator response = new();
            response.SetBody("test header 2");
            serverStruct.Response = response.GetResponse();
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
            ResponseCreator response = new();
            response.SetHeaders(new Dictionary<string, string>() 
            {
                { "key", "Value" }
            });
            response.SetBody("this is a proper test, and making things");
            serverStruct.Response = response.GetResponse();
            ResponseSender.SendResponse(serverStruct);
            return true;
        }


        [HTTP("GET", "/test3/{test}/test?aa=yes&asd={xx}")]
        public static bool test3(HttpRequest request, ServerStruct serverStruct)
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
            ResponseCreator response = new();
            response.SetHeaders(new Dictionary<string, string>()
            {
                { "key", "Value" }
            });
            response.SetBody("this is a proper test, and making things");
            serverStruct.Response = response.GetResponse();
            ResponseSender.SendResponse(serverStruct);
            return true;
        }

        [HTTP("GET", "/test3/{test}/test2?{args}")]
        public static bool test3args(HttpRequest request, ServerStruct serverStruct)
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
            ResponseCreator response = new();
            response.SetHeaders(new Dictionary<string, string>()
            {
                { "key", "Value" }
            });
            response.SetBody("this is a proper test, and making things");
            serverStruct.Response = response.GetResponse();
            ResponseSender.SendResponse(serverStruct);
            return true;
        }


        [HTTP("GET", "/test3/{test}?{args}")]
        public static bool test3fullargs(HttpRequest request, ServerStruct serverStruct)
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
            ResponseCreator response = new();
            response.SetHeaders(new Dictionary<string, string>()
            {
                { "key", "Value" }
            });
            response.SetBody("this is a proper test, and making things");
            serverStruct.Response = response.GetResponse();
            ResponseSender.SendResponse(serverStruct);
            return true;
        }

    }

}
#endif
