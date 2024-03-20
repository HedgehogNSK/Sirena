using Hedgey.Structure.Factory;
using MongoDB.Driver;

namespace Hedgey.Sirena;

public class MongoClientFactory : IFactory<MongoClient>
{
  public MongoClient Create()
  {
    var connectionString = Environment.GetEnvironmentVariable("MONGODB_URI");
    if (connectionString == null)
    {
      Console.WriteLine("You must set your 'MONGODB_URI' environment variable. To learn how to set it, see https://www.mongodb.com/docs/drivers/csharp/current/quick-start/#set-your-connection-string");
      Environment.Exit(0);
    }
    return new MongoClient(connectionString);
  }
}