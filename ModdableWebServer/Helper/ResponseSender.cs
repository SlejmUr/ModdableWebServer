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
                    break;
                case ServerEnum.HTTPS:
                    serverStruct.HTTPS_Session?.SendResponse(serverStruct.Response);
                    break;
                case ServerEnum.WS:
                    serverStruct.WS_Session?.SendResponse(serverStruct.Response);
                    break;
                case ServerEnum.WSS:
                    serverStruct.WSS_Session?.SendResponse(serverStruct.Response);
                    break;
                default:
                    break;
            }

        }
    }
}
