using Hedgey.Extensions.SimpleInjector;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Bot.Operations.Mongo;
using Hedgey.Sirena.Database;
using MongoDB.Driver;
using SimpleInjector;

namespace Hedgey.Sirena.Bot.DI;

public class SharedCommandServicesInstaller(Container container) : Installer(container)
{
  public override void Install()
  {
    Container.RegisterSingleton<IDeleteSirenaOperation, SirenaOperations>();
    Container.RegisterSingleton<IFindSirenaOperation, SirenaOperations>();
    Container.RegisterSingleton<IGetUserRelatedSirenas, SirenaOperations>();
    Container.RegisterSingleton<ISubscribeToSirenaOperation, SirenaOperations>();
    Container.RegisterSingleton<IUpdateSirenaOperation, SirenaOperations>();
    Container.RegisterSingleton<IUnsubscribeSirenaOperation, SirenaOperations>();
    Container.RegisterSingleton<IGetUserInformation, GetUserInformation>();
    Container.RegisterSingleton<IGetUserOverviewAsync, GetUserStatsOperationAsync>();

    Container.RegisterSingleton<FacadeMongoDBRequests>();
    Container.RegisterSingleton<MongoClient>(()=> new MongoClient());//MongoClientFactory connection settings to db
    Container.RegisterSingleton<IMongoDatabase>(() => Container.GetInstance<MongoClient>().GetDatabase("siren"));//MongoClientFactory connection settings to db
    Container.RegisterSingleton<IMongoCollection<SirenRepresentation>>(()
      => Container.GetInstance<IMongoDatabase>().GetCollection<SirenRepresentation>("sirens"));
    Container.RegisterSingleton<IMongoCollection<UserRepresentation>>(()
      => Container.GetInstance<IMongoDatabase>().GetCollection<UserRepresentation>("users"));
  }
}