using NetCoreServer;

namespace ModdableWebServer.Helper
{
    public static class ResponseSender
    {
        public static void SendResponse(this ServerStruct serverStruct)
        {
            switch (serverStruct.Enum)
            {
                case ServerEnum.HTTP:
                    serverStruct.HTTP_Session?.SendResponse(serverStruct.Response);
                    DebugPrinter.Debug("[SendResponse] HTTP Response Sent!");
                    break;
                case ServerEnum.HTTPS:
                    serverStruct.HTTPS_Session?.SendResponse(serverStruct.Response);
                    DebugPrinter.Debug("[SendResponse] HTTPS Response Sent!");
                    break;
                case ServerEnum.WS:
                    serverStruct.WS_Session?.SendResponse(serverStruct.Response);
                    DebugPrinter.Debug("[SendResponse] WS Response Sent!");
                    break;
                case ServerEnum.WSS:
                    serverStruct.WSS_Session?.SendResponse(serverStruct.Response);
                    DebugPrinter.Debug("[SendResponse] WSS Response Sent!");
                    break;
                default:
                    break;
            }
        }

        public static void SendResponse(this ServerStruct serverStruct, HttpResponse response)
        {
            switch (serverStruct.Enum)
            {
                case ServerEnum.HTTP:
                    serverStruct.HTTP_Session?.SendResponse(response);
                    DebugPrinter.Debug("[SendResponse] HTTP Response Sent!");
                    break;
                case ServerEnum.HTTPS:
                    serverStruct.HTTPS_Session?.SendResponse(response);
                    DebugPrinter.Debug("[SendResponse] HTTPS Response Sent!");
                    break;
                case ServerEnum.WS:
                    serverStruct.WS_Session?.SendResponse(response);
                    DebugPrinter.Debug("[SendResponse] WS Response Sent!");
                    break;
                case ServerEnum.WSS:
                    serverStruct.WSS_Session?.SendResponse(response);
                    DebugPrinter.Debug("[SendResponse] WSS Response Sent!");
                    break;
                default:
                    break;
            }
        }
    }
}
