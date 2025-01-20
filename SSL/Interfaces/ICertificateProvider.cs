using System.Security.Cryptography.X509Certificates;

namespace Hedgey.Security.x509Certificates;
public interface ICertificateProvider
{
  X509Certificate2 Get();
}