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

    public static Guid GetId(this ServerStruct serverStruct)
    {
        return serverStruct.Enum switch
        {
            ServerEnum.HTTP => serverStruct.HTTP_Session!.Id,
            ServerEnum.HTTPS => serverStruct.HTTPS_Session!.Id,
            ServerEnum.WS => serverStruct.WS_Session!.Id,
            ServerEnum.WSS => serverStruct.WSS_Session!.Id,
            _ => throw new NotImplementedException("[GetId] ServerEnum type not exist!")
        };
    }
}
