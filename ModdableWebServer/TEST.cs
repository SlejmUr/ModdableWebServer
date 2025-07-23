#if DEBUG
using ModdableWebServer.Attributes;
using ModdableWebServer.Helper;
using ModdableWebServer.Senders;

namespace ModdableWebServer;

public class TEST
{

    [WS("/ws/{test}",  WebSocketMethodListen.All)]
    public static void Ws(WebSocketSender sender)
    {
        Console.WriteLine("Headers:");
        foreach (var item in sender.Headers)
        {
            Console.WriteLine(item.Key + " = " + item.Value);
        }
        Console.WriteLine("Parameters:");
        foreach (var item in sender.Parameters)
        {
            Console.WriteLine(item.Key + " = " + item.Value);
        }
        Console.WriteLine("Current Listening: " + sender.CurrentMethod);
        if (sender.CurrentMethod.HasFlag(WebSocketMethodListen.Close))
        {
            Console.WriteLine("CloseStatus: " + sender.CloseStatus);
        }
        if (sender.CurrentMethod > WebSocketMethodListen.Received)
        {
            Console.WriteLine(sender.Buffer.Length);
            Console.WriteLine(sender.Offset);
            Console.WriteLine(sender.Size);
        }
    }



    [HTTP("GET", "/test")]
    [HTTP("GET", "/test1")]
    [HTTP("GET", "/test11")]
    public static bool TEST1(ServerSender sender)
    {
        Console.WriteLine("Headers:");
        foreach (var item in sender.Headers)
        {
            Console.WriteLine(item.Key + " = " + item.Value);
        }
        Console.WriteLine("Parameters:");
        foreach (var item in sender.Parameters)
        {
            Console.WriteLine(item.Key + " = " + item.Value);
        }

        Console.WriteLine("TEST");
        sender.Response.MakeOkResponse();
        sender.SendResponse();
        return true;
    }



    [HTTP("GET", "/{!args}")]
    public static bool Lol(ServerSender sender)
    {
        Console.WriteLine("Headers:");
        foreach (var item in sender.Headers)
        {
            Console.WriteLine(item.Key + " = " + item.Value);
        }
        Console.WriteLine("Parameters:");
        foreach (var item in sender.Parameters)
        {
            Console.WriteLine(item.Key + " = " + item.Value);
        }

        Console.WriteLine("!ARGS");
        sender.Response.MakeOkResponse();
        sender.SendResponse();
        return true;
    }


    [HTTP("DELETE", "/del")]
    public static bool DEL(ServerSender sender)
    {
        Console.WriteLine("Headers:");
        foreach (var item in sender.Headers)
        {
            Console.WriteLine(item.Key + " = " + item.Value);
        }
        Console.WriteLine("Parameters:");
        foreach (var item in sender.Parameters)
        {
            Console.WriteLine(item.Key + " = " + item.Value);
        }

        Console.WriteLine("TEST");
        ResponseCreator response = new(200);
        response.SetBody("balls");
        sender.SendResponse(response);
        return true;
    }

    [HTTPHeader("GET", "/testheader2", "test")]
    [HTTPHeader("GET", "/testheader", "test")]
    public static bool TEST_HEADER(ServerSender sender)
    {
        Console.WriteLine("Headers:");
        foreach (var item in sender.Headers)
        {
            Console.WriteLine(item.Key + " = " + item.Value);
        }
        Console.WriteLine("Parameters:");
        foreach (var item in sender.Parameters)
        {
            Console.WriteLine(item.Key + " = " + item.Value);
        }

        Console.WriteLine("TEST HEADER");
        ResponseCreator response = new();
        response.SetBody("test header");
        sender.SendResponse(response);
        return true;
    }

    // IF we would use the same url we would have been down, since the other one is triggered before this one.
    [HTTPHeader("GET", "/testheader2", "test", "myvalue")]
    [HTTPHeader("GET", "/testheader2", "test2", "jtw.", true)]
    public static bool TEST_HEADER2(ServerSender sender)
    {
        Console.WriteLine("Headers:");
        foreach (var item in sender.Headers)
        {
            Console.WriteLine(item.Key + " = " + item.Value);
        }
        Console.WriteLine("Parameters:");
        foreach (var item in sender.Parameters)
        {
            Console.WriteLine(item.Key + " = " + item.Value);
        }

        Console.WriteLine("TEST HEADER 2");
        ResponseCreator response = new();
        response.SetBody("test header 2");
        sender.SendResponse(response);
        return true;
    }


    [HTTP("GET", "/test2/{test}")]
    public static bool Test2(ServerSender sender)
    {
        Console.WriteLine("Headers:");
        foreach (var item in sender.Headers)
        {
            Console.WriteLine(item.Key + " = " + item.Value);
        }
        Console.WriteLine("Parameters:");
        foreach (var item in sender.Parameters)
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
        sender.SendResponse(response);
        return true;
    }


    [HTTP("GET", "/test3/{test}/test?aa=yes&asd={xx}")]
    public static bool Test3(ServerSender sender)
    {
        Console.WriteLine("Headers:");
        foreach (var item in sender.Headers)
        {
            Console.WriteLine(item.Key + " = " + item.Value);
        }
        Console.WriteLine("Parameters:");
        foreach (var item in sender.Parameters)
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
        sender.SendResponse(response);
        return true;
    }

    [HTTP("GET", "/test3/{test}/test2?{!args}")]
    public static bool Test3args(ServerSender sender)
    {
        Console.WriteLine("Headers:");
        foreach (var item in sender.Headers)
        {
            Console.WriteLine(item.Key + " = " + item.Value);
        }
        Console.WriteLine("Parameters:");
        foreach (var item in sender.Parameters)
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
        sender.SendResponse(response);
        return true;
    }


    [HTTP("GET", "/test3/{test}?{!args}")]
    public static bool Test3fullargs(ServerSender sender)
    {
        Console.WriteLine("Headers:");
        foreach (var item in sender.Headers)
        {
            Console.WriteLine(item.Key + " = " + item.Value);
        }
        Console.WriteLine("Parameters:");
        foreach (var item in sender.Parameters)
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
        sender.SendResponse(response);
        return true;
    }

}
#endif
