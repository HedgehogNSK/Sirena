using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Hedgey.Security.x509Certificates;

public class X509StoreActiveHttpsCertificateProvider(StoreName storeName, StoreLocation storeLocation) : ICertificateProvider
{
  private readonly StoreName storeName = storeName;
  private readonly StoreLocation storeLocation = storeLocation;

  public X509Certificate2 Get()
  {
    using var store = new X509Store(storeName, storeLocation);
    store.Open(OpenFlags.ReadOnly);
    var envValue = Environment.GetEnvironmentVariable("SIRENA_ALLOW_LOCALHOST");
    bool isLocalAllowed = envValue != null && bool.TryParse(envValue, out var parsed) && parsed;
    var now = DateTime.UtcNow;
    var certificate = store.Certificates
        .Cast<X509Certificate2>()
        .Where(cert => cert.NotBefore <= now && cert.NotAfter >= now &&
          (isLocalAllowed || !IsLocalhostCertificate(cert)) &&
          cert.Extensions.OfType<X509EnhancedKeyUsageExtension>()
            .Any(ext => ext.EnhancedKeyUsages.Cast<Oid>().Any(oid => oid.Value == "1.3.6.1.5.5.7.3.1"))) // проверка HTTPS (Server Authentication)
        .OrderByDescending(cert => (cert.NotAfter - now).TotalDays)
        .FirstOrDefault();

    if (certificate == null)
      throw new FileNotFoundException($"No active HTTPS certificate found in store {storeName} / {storeLocation}");

    return certificate;
  }
  bool IsLocalhostCertificate(X509Certificate2 cert)
  {
    if (cert.Subject.Contains("CN=localhost", StringComparison.OrdinalIgnoreCase))
      return true;

    foreach (var ext in cert.Extensions.OfType<X509Extension>())
    {
      if (ext.Oid?.Value == "2.5.29.17") // Subject Alternative Name
      {
        var asnData = new AsnEncodedData(ext.Oid, ext.RawData);
        var san = asnData.Format(true);
        if (san.Contains("DNS Name=localhost", StringComparison.OrdinalIgnoreCase))
          return true;
      }
    }
    return false;
  }
}
