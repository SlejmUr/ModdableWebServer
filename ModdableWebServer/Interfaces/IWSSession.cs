namespace ModdableWebServer.Interfaces;

public interface IWSSession : IHttpSession
{
    bool Close();
    bool Close(int status);
    bool Close(int status, ReadOnlySpan<byte> buffer);

    long SendText(ReadOnlySpan<char> text);
    long SendText(ReadOnlySpan<byte> buffer);

    long SendBinary(ReadOnlySpan<char> text);
    long SendBinary(ReadOnlySpan<byte> buffer);

    long SendPing(ReadOnlySpan<char> text);
    long SendPing(ReadOnlySpan<byte> buffer);

    long SendPong(ReadOnlySpan<char> text);
    long SendPong(ReadOnlySpan<byte> buffer);

    long SendClose(int status, ReadOnlySpan<char> text);
    long SendClose(int status, ReadOnlySpan<byte> buffer);
}

