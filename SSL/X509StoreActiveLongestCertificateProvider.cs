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

        var now = DateTime.UtcNow;
        var certificate = store.Certificates
            .Cast<X509Certificate2>()
            .Where(cert => cert.NotBefore <= now && cert.NotAfter >= now &&
              cert.Extensions.OfType<X509EnhancedKeyUsageExtension>()
                .Any(ext => ext.EnhancedKeyUsages.Cast<Oid>().Any(oid => oid.Value == "1.3.6.1.5.5.7.3.1"))) // проверка HTTPS (Server Authentication)
            .OrderByDescending(cert => (cert.NotAfter - now).TotalDays)
            .FirstOrDefault();

        if (certificate == null)
            throw new FileNotFoundException($"No active HTTPS certificate found in store {storeName} / {storeLocation}");

        return certificate;
    }
}
