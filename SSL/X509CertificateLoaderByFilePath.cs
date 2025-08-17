using System.Security.Cryptography.X509Certificates;

namespace Hedgey.Security.x509Certificates;

public class X509CertificateLoaderByFilePath(string fullchainPath, string keyPath) : ICertificateProvider
{
  public X509Certificate2 Get()
    => X509Certificate2.CreateFromPemFile(fullchainPath, keyPath);
}