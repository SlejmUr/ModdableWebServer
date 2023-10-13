namespace ModdableWebServer.Helper
{
    public class ResponseSender
    {
        public static void SendResponse(ServerStruct serverStruct)
        {
            switch (serverStruct.Enum)
            {
                case ServerEnum.HTTP:
                    serverStruct.HTTP_Session?.SendResponse(serverStruct.Response);
                    break;
                case ServerEnum.HTTPS:
                    break;
                case ServerEnum.WS:
                    serverStruct.WS_Session?.SendResponse(serverStruct.Response);
                    break;
                case ServerEnum.WSS:
                    break;
                default:
                    break;
            }

        }
    }
}
