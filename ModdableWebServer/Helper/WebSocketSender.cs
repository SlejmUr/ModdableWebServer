namespace ModdableWebServer.Helper
{
    public static class WebSocketSender
    {
        public static void SendWebSocketText(this WebSocketStruct socketStruct, string text)
        {
            if (socketStruct.WSRequest != null)
            {
                switch (socketStruct.Enum)
                {
                    case WSEnum.WS:
                        socketStruct.WS_Session?.SendText(text);
                        break;
                    case WSEnum.WSS:
                        socketStruct.WSS_Session?.SendText(text);
                        break;
                    default:
                        break;
                }
            }
        }

        public static void SendWebSocketByteArray(this WebSocketStruct socketStruct, byte[] bytes)
        {
            if (socketStruct.WSRequest != null)
            {
                switch (socketStruct.Enum)
                {
                    case WSEnum.WS:
                        socketStruct.WS_Session?.SendBinary(bytes);
                        break;
                    case WSEnum.WSS:
                        socketStruct.WSS_Session?.SendBinary(bytes);
                        break;
                    default:
                        break;
                }
            }
        }

        public static void SendWebSocketClose(this WebSocketStruct socketStruct, int status, string text)
        {
            if (socketStruct.WSRequest != null)
            {
                switch (socketStruct.Enum)
                {
                    case WSEnum.WS:
                        socketStruct.WS_Session?.SendClose(status, text);
                        break;
                    case WSEnum.WSS:
                        socketStruct.WSS_Session?.SendClose(status, text);
                        break;
                    default:
                        break;
                }
            }
        }

        public static void MulticastWebSocketText(this WebSocketStruct socketStruct, int status, string text)
        {
            if (socketStruct.WSRequest != null)
            {
                switch (socketStruct.Enum)
                {
                    case WSEnum.WS:
                        socketStruct.WS_Session?.WS_Server.MulticastText(text);
                        break;
                    case WSEnum.WSS:
                        socketStruct.WSS_Session?.WSS_Server.MulticastText(text);
                        break;
                    default:
                        break;
                }
            }
        }

        public static void MulticastWebSocketBinary(this WebSocketStruct socketStruct, int status, byte[] bytes)
        {
            if (socketStruct.WSRequest != null)
            {
                switch (socketStruct.Enum)
                {
                    case WSEnum.WS:
                        socketStruct.WS_Session?.WS_Server.MulticastBinary(bytes);
                        break;
                    case WSEnum.WSS:
                        socketStruct.WSS_Session?.WSS_Server.MulticastBinary(bytes);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
