using NetCoreServer;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace ModdableWebServer.Helper;

public class CertHelper
{
    public static X509Certificate GetCert(string pfxPath, string password)
    {
        if (string.IsNullOrEmpty(pfxPath))
            throw new ArgumentNullException(nameof(pfxPath));

        if (!File.Exists(pfxPath))
            throw new FileNotFoundException($"{pfxPath} File not found");

        return new X509Certificate2(File.ReadAllBytes(pfxPath), password);
    }

    public static X509Certificate GetCertPem(string certPath, string keyPath)
    {
        if (string.IsNullOrEmpty(certPath))
            throw new ArgumentNullException(nameof(certPath));

        if (string.IsNullOrEmpty(keyPath))
            throw new ArgumentNullException(nameof(keyPath));

        if (!File.Exists(certPath))
            throw new FileNotFoundException($"{certPath} File not found");

        if (!File.Exists(keyPath))
            throw new FileNotFoundException($"{keyPath} File not found");

        return new X509Certificate2(X509Certificate2.CreateFromPemFile(certPath, keyPath).Export(X509ContentType.Pfx));
    }

    public static SslContext GetContext(SslProtocols sslprotocol, string pfxPath, string password)
    {
        return new SslContext(sslprotocol, GetCert(pfxPath, password));
    }

    public static SslContext GetContextNoValidate(SslProtocols sslprotocol, string pfxPath, string password)
    {
        return new SslContext(sslprotocol, GetCert(pfxPath, password), new RemoteCertificateValidationCallback(NoCertificateValidator));
    }

    public static SslContext GetContextPem(SslProtocols sslprotocol, string pfxPath, string password)
    {
        return new SslContext(sslprotocol, GetCertPem(pfxPath, password));
    }
    public static SslContext GetContextPamNoValidate(SslProtocols sslprotocol, string pfxPath, string password)
    {
        return new SslContext(sslprotocol, GetCertPem(pfxPath, password), new RemoteCertificateValidationCallback(NoCertificateValidator));
    }

    public static bool NoCertificateValidator(object? sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }
}
