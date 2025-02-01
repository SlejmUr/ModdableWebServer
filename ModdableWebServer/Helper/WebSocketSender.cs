namespace ModdableWebServer.Helper;

public static class WebSocketSender
{
    public static void SendWebSocketText(this WebSocketStruct socketStruct, string text)
    {
        if (!socketStruct.IsConnected)
            return;
        switch (socketStruct.Enum)
        {
            case WSEnum.WS:
                socketStruct.WS_Session?.SendText(text);
                DebugPrinter.Debug("[SendWebSocketText] WS Text Sent!");
                break;
            case WSEnum.WSS:
                socketStruct.WSS_Session?.SendText(text);
                DebugPrinter.Debug("[SendWebSocketText] WSS Text Sent!");
                break;
            default:
                break;
        }
    }

    public static void SendWebSocketByteArray(this WebSocketStruct socketStruct, byte[] bytes)
    {
        if (!socketStruct.IsConnected)
            return;
        switch (socketStruct.Enum)
        {
            case WSEnum.WS:
                socketStruct.WS_Session?.SendBinary(bytes);
                DebugPrinter.Debug("[SendWebSocketByteArray] WS Binary Sent!");
                break;
            case WSEnum.WSS:
                socketStruct.WSS_Session?.SendBinary(bytes);
                DebugPrinter.Debug("[SendWebSocketByteArray] WSS Binary Sent!");
                break;
            default:
                break;
        }
    }

    public static void SendWebSocketClose(this WebSocketStruct socketStruct, int status, string text)
    {
        if (!socketStruct.IsConnected)
            return;
        switch (socketStruct.Enum)
        {
            case WSEnum.WS:
                socketStruct.WS_Session?.SendClose(status, text);
                DebugPrinter.Debug("[SendWebSocketClose] WS Close Sent!");
                break;
            case WSEnum.WSS:
                socketStruct.WSS_Session?.SendClose(status, text);
                DebugPrinter.Debug("[SendWebSocketClose] WSS Close Sent!");
                break;
            default:
                break;
        }
    }

    public static void MulticastWebSocketText(this WebSocketStruct socketStruct, string text)
    {
        if (!socketStruct.IsConnected)
            return;
        switch (socketStruct.Enum)
        {
            case WSEnum.WS:
                socketStruct.WS_Session?.WS_Server.MulticastText(text);
                DebugPrinter.Debug("[MulticastWebSocketText] WS Multicast Text Sent!");
                break;
            case WSEnum.WSS:
                socketStruct.WSS_Session?.WSS_Server.MulticastText(text);
                DebugPrinter.Debug("[MulticastWebSocketText] WSS Multicast Text Sent!");
                break;
            default:
                break;
        }
    }

    public static void MulticastWebSocketBinary(this WebSocketStruct socketStruct, byte[] bytes)
    {
        if (!socketStruct.IsConnected)
            return;
        switch (socketStruct.Enum)
        {
            case WSEnum.WS:
                socketStruct.WS_Session?.WS_Server.MulticastBinary(bytes);
                DebugPrinter.Debug("[MulticastWebSocketBinary] WS Multicast Binary Sent!");
                break;
            case WSEnum.WSS:
                socketStruct.WSS_Session?.WSS_Server.MulticastBinary(bytes);
                DebugPrinter.Debug("[MulticastWebSocketBinary] WSS Multicast Binary Sent!");
                break;
            default:
                break;
        }
    }
}
