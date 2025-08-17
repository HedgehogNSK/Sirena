using Hedgey.Extensions;
using System.Security.Cryptography.X509Certificates;

namespace Hedgey.Security.x509Certificates;
public class X509StoreCertificateProvider(StoreName storeName, StoreLocation storeLocation) : ICertificateProvider
{
  private readonly StoreName storeName = storeName;
  private readonly StoreLocation storeLocation = storeLocation;

  public X509Certificate2 Get()
  {
    var store = new X509Store(storeName, storeLocation);

    store.Open(OpenFlags.ReadOnly);
    string thumbnail = OSTools.GetEnvironmentVar("HTTPS_CERT_THUMBNAIL");
    var certificates = store.Certificates.Find(X509FindType.FindByThumbprint, thumbnail, false);
    if (certificates.Count == 0)
      throw new FileNotFoundException($"Certificate with thumbnail:\'{thumbnail}\' hasn't been found");
    var certificate = certificates[0];

    store.Close();
    return certificate;
  }
}