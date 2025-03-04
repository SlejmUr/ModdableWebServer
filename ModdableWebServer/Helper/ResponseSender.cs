using NetCoreServer;
using System.Reflection;

namespace ModdableWebServer.Helper;

public static class ResponseSender
{
    public static void SendResponse(this ServerStruct serverStruct)
    {
        switch (serverStruct.Enum)
        {
            case ServerEnum.HTTP:
                serverStruct.HTTP_Session?.GetType().GetRuntimeMethod("SendResponse", [typeof(HttpResponse)])?.Invoke(serverStruct.HTTP_Session, [serverStruct.Response]);
                DebugPrinter.Debug("[SendResponse] HTTP Response Sent!");
                break;
            case ServerEnum.HTTPS:
                serverStruct.HTTPS_Session?.GetType().GetRuntimeMethod("SendResponse", [typeof(HttpResponse)])?.Invoke(serverStruct.HTTPS_Session, [serverStruct.Response]);
                DebugPrinter.Debug("[SendResponse] HTTPS Response Sent!");
                break;
            case ServerEnum.WS:
                serverStruct.WS_Session?.GetType().GetRuntimeMethod("SendResponse", [typeof(HttpResponse)])?.Invoke(serverStruct.WS_Session, [serverStruct.Response]);
                DebugPrinter.Debug("[SendResponse] WS Response Sent!");
                break;
            case ServerEnum.WSS:
                serverStruct.WSS_Session?.GetType().GetRuntimeMethod("SendResponse", [typeof(HttpResponse)])?.Invoke(serverStruct.WSS_Session, [serverStruct.Response]);
                DebugPrinter.Debug("[SendResponse] WSS Response Sent!");
                break;
            default:
                break;
        }
    }

    public static void SendResponse(this ServerStruct serverStruct, HttpResponse response)
    {
        serverStruct.Response = response;
        SendResponse(serverStruct);
    }
}
