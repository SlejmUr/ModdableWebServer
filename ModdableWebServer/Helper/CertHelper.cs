using NetCoreServer;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace ModdableWebServer.Helper
{
    public class CertHelper
    {
        public static X509Certificate GetCert(string pfxPath, string password)
        {
            if (pfxPath == null)
                throw new ArgumentNullException(nameof(pfxPath));

            if (!File.Exists(pfxPath))
                throw new FileNotFoundException($"{pfxPath} File not found");

            return new X509Certificate2(File.ReadAllBytes(pfxPath), password);
        }

        public static SslContext GetContext(SslProtocols sslprotocol, string pfxPath, string password)
        {
            return new SslContext(sslprotocol, GetCert(pfxPath, password));
        }
        public static SslContext GetContextNoValidate(SslProtocols sslprotocol, string pfxPath, string password)
        {
            return new SslContext(sslprotocol, GetCert(pfxPath, password), new RemoteCertificateValidationCallback(NoCertificateValidator));
        }

        public static bool NoCertificateValidator(object? sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
