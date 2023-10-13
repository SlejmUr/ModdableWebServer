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
    }
}
