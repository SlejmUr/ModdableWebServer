using System.Net.Sockets;

namespace ModdableWebServer.Helper;

public static class SocketHelper
{
    public static Socket GetSocket(this ServerStruct serverStruct)
    {
        return serverStruct.Enum switch
        {
            ServerEnum.HTTP => serverStruct.HTTP_Session!.Socket,
            ServerEnum.HTTPS => serverStruct.HTTPS_Session!.Socket,
            ServerEnum.WS => serverStruct.WS_Session!.Socket,
            ServerEnum.WSS => serverStruct.WSS_Session!.Socket,
            _ => throw new NotImplementedException("[GetSocket] ServerEnum type not exist!")
        };
    }
}
