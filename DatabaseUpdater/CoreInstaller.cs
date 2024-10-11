using Hedgey.Extensions;
using Hedgey.Extensions.SimpleInjector;
using Hedgey.Sirena;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Bot.Operations.Mongo;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using MongoDB.Driver;
using SimpleInjector;

namespace DatabaseUpdater
{
  public class CoreInstaller(Container container)
  : Installer(container)
  {
    const int MACHINE_ID = 0;
    //DO NOT TOUCH THIS STAMP
    const long EPOCH_TIMESTAMP = 1729300000000;  //Mon Oct 14 2024 22:13:20 GMT+0000
    public override void Install()
    {
      Container.RegisterSingleton<IIDGenerator>(() => new BlendedflakeIDGenerator(EPOCH_TIMESTAMP, MACHINE_ID));
      Container.RegisterSingleton<IUpdateSirenaOperation, SirenaOperations>();
      
      Container.Register<IFactory<IMongoClient>, MongoClientFactory>(Lifestyle.Transient);
    Container.RegisterSingleton<IMongoClient>(() => Container.GetInstance<IFactory<IMongoClient>>().Create());
    Container.RegisterSingleton<IMongoDatabase>(() => Container.GetInstance<IMongoClient>().GetDatabase("siren"));
    Container.RegisterSingleton<IMongoCollection<SirenRepresentation>>(()
      => Container.GetInstance<IMongoDatabase>().GetCollection<SirenRepresentation>("sirens"));
    Container.RegisterSingleton<IMongoCollection<UserRepresentation>>(()
      => Container.GetInstance<IMongoDatabase>().GetCollection<UserRepresentation>("users"));
    }
  }
}