namespace Hedgey.Security.x509Certificates;

using System;
using System.Security.Cryptography.X509Certificates;

public static class Utilities
{

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

      Console.WriteLine("Certificate is invalid. Errors:");

      foreach (var status in chain.ChainStatus)
        Console.WriteLine($"  {status.Status}: {status.StatusInformation}");

      return false;
    }
  }
}