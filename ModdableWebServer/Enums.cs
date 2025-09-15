namespace ModdableWebServer;

[Flags]
public enum WebSocketMethodListen
{
    None,
    Connected = 1,
    Connecting = 2,
    Disconnected = 4,
    Disconnecting = 8,
    Received = 16,
    Ping = 32,
    Pong = 64,
    Close = 128,
    All = Connected | Connecting | Disconnected | Disconnecting | Received | Ping | Pong | Close,
}