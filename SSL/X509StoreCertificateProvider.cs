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

  public static bool Validate(X509Certificate2 certificate)
  {
    using (var chain = new X509Chain())
    {
      // Setting flag to check certificate
      chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;

      // Building certificates chain
      bool isValid = chain.Build(certificate);

      if (isValid)
      {
        Console.WriteLine("Certificate is valid");
        return true;
      }

      Console.WriteLine("Certificate is invalid. Erros:");

      foreach (var status in chain.ChainStatus)
        Console.WriteLine($"  {status.Status}: {status.StatusInformation}");
        
      return false;
    }
  }
}