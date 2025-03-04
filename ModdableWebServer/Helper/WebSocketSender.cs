using System.Reflection;

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
                socketStruct.WS_Session?.Server.GetType().GetRuntimeMethod("SendText", [typeof(string)])?.Invoke(socketStruct.WS_Session?.Server, [text]);
                DebugPrinter.Debug("[SendWebSocketText] WS Text Sent!");
                break;
            case WSEnum.WSS:
                socketStruct.WSS_Session?.Server.GetType().GetRuntimeMethod("SendText", [typeof(string)])?.Invoke(socketStruct.WSS_Session?.Server, [text]);
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
                socketStruct.WS_Session?.Server.GetType().GetRuntimeMethod("SendBinary", [typeof(byte[])])?.Invoke(socketStruct.WS_Session?.Server, [bytes]);
                DebugPrinter.Debug("[SendWebSocketByteArray] WS Binary Sent!");
                break;
            case WSEnum.WSS:
                socketStruct.WSS_Session?.Server.GetType().GetRuntimeMethod("SendBinary", [typeof(byte[])])?.Invoke(socketStruct.WSS_Session?.Server, [bytes]);
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
                socketStruct.WS_Session?.Server.GetType().GetRuntimeMethod("SendClose", [typeof(int),typeof(string)])?.Invoke(socketStruct.WS_Session?.Server, [status, text]);
                DebugPrinter.Debug("[SendWebSocketClose] WS Close Sent!");
                break;
            case WSEnum.WSS:
                socketStruct.WSS_Session?.Server.GetType().GetRuntimeMethod("SendClose", [typeof(int), typeof(string)])?.Invoke(socketStruct.WSS_Session?.Server, [status, text]);
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
                socketStruct.WS_Session?.Server.GetType().GetRuntimeMethod("MulticastText", [typeof(string)])?.Invoke(socketStruct.WS_Session?.Server, [text]);
                DebugPrinter.Debug("[MulticastWebSocketText] WS Multicast Text Sent!");
                break;
            case WSEnum.WSS:
                socketStruct.WSS_Session?.Server.GetType().GetRuntimeMethod("MulticastText", [typeof(string)])?.Invoke(socketStruct.WSS_Session?.Server, [text]);
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
                socketStruct.WS_Session?.Server.GetType().GetRuntimeMethod("MulticastBinary", [typeof(byte[])])?.Invoke(socketStruct.WS_Session?.Server, [bytes]);
                DebugPrinter.Debug("[MulticastWebSocketBinary] WS Multicast Binary Sent!");
                break;
            case WSEnum.WSS:
                socketStruct.WSS_Session?.Server.GetType().GetRuntimeMethod("MulticastBinary", [typeof(byte[])])?.Invoke(socketStruct.WSS_Session?.Server, [bytes]);
                DebugPrinter.Debug("[MulticastWebSocketBinary] WSS Multicast Binary Sent!");
                break;
            default:
                break;
        }
    }
}
