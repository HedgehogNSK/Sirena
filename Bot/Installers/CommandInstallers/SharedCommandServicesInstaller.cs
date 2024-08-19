using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Bot.Operations.Mongo;
using Hedgey.Sirena.Database;
using MongoDB.Driver;
using SimpleInjector;

namespace Hedgey.Sirena.Bot;

public class SharedCommandServicesInstaller(Container container) : Installer(container)
{
  public override void Install()
  {
    Container.Register<IDeleteSirenaOperation, SirenaOperations>();
    Container.Register<IFindSirenaOperation, SirenaOperations>();
    Container.Register<IGetUserRelatedSirenas, SirenaOperations>();
    Container.Register<ISubscribeToSirenaOperation, SirenaOperations>();
    Container.Register<IUpdateSirenaOperation, SirenaOperations>();
    Container.Register<IUnsubscribeSirenaOperation, SirenaOperations>();
    Container.Register<IGetUserInformation, GetUserInformation>();
    Container.Register<IGetUserOverviewAsync, GetUserStatsOperationAsync>();

    Container.RegisterSingleton<FacadeMongoDBRequests>();
    Container.RegisterSingleton<MongoClient>(()=> new MongoClient());//MongoClientFactory connection settings to db
    Container.RegisterSingleton<IMongoDatabase>(() => Container.GetInstance<MongoClient>().GetDatabase("siren"));//MongoClientFactory connection settings to db
    Container.RegisterSingleton<IMongoCollection<SirenRepresentation>>(()
      => Container.GetInstance<IMongoDatabase>().GetCollection<SirenRepresentation>("sirens"));
    Container.RegisterSingleton<IMongoCollection<UserRepresentation>>(()
      => Container.GetInstance<IMongoDatabase>().GetCollection<UserRepresentation>("users"));
  }
}