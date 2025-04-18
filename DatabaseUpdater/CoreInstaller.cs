using Hedgey.Extensions.Blendflake;
using Hedgey.Extensions.SimpleInjector;
using Hedgey.Sirena;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.MongoDB.Operations;
using Hedgey.Sirena.Entities;
using Hedgey.Sirena.ID;
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
    const long EPOCH_TIMESTAMP = 1736959000000;  //Wed Jan 15 2025 16:36:40 GMT+0000
    public override void Install()
    {
      Container.RegisterSingleton<IIDGenerator>(() => new BlendflakeAdapter(EPOCH_TIMESTAMP, MACHINE_ID));
      Container.RegisterSingleton<IUpdateSirenaOperation, SirenaOperations>();

      Container.Register<IFactory<IMongoClient>, MongoClientFactory>(Lifestyle.Transient);
      Container.RegisterSingleton<IMongoClient>(() => Container.GetInstance<IFactory<IMongoClient>>().Create());
      Container.RegisterSingleton<IMongoDatabase>(() => Container.GetInstance<IMongoClient>().GetDatabase("siren"));
      Container.RegisterSingleton<IMongoCollection<SirenaData>>(()
      => Container.GetInstance<IMongoDatabase>().GetCollection<SirenaData>("sirens"));
      Container.RegisterSingleton<IMongoCollection<UserData>>(()
        => Container.GetInstance<IMongoDatabase>().GetCollection<UserData>("users"));
    }
  }
}