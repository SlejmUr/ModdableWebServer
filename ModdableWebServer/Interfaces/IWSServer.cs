using ModdableWebServer.Attributes;
using System.Reflection;

namespace ModdableWebServer.Interfaces;

public interface IWSServer : IHttpServer
{
    Dictionary<WSAttribute, MethodInfo> WSAttributeToMethods { get; }

    bool CloseAll();
    bool CloseAll(int status);
    bool CloseAll(int status, ReadOnlySpan<byte> buffer);

    bool MulticastText(ReadOnlySpan<char> text);
    bool MulticastText(ReadOnlySpan<byte> buffer);

    bool MulticastBinary(ReadOnlySpan<char> text);
    bool MulticastBinary(ReadOnlySpan<byte> buffer);

    bool MulticastPing(ReadOnlySpan<char> text);
    bool MulticastPing(ReadOnlySpan<byte> buffer);
}
