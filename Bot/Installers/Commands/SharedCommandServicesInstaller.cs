using Hedgey.Extensions.SimpleInjector;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Bot.Operations.Mongo;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
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
    Container.RegisterSingleton<IRightsManageOperation, SirenaOperations>();
    Container.RegisterSingleton<IGetUserOverviewAsync, UserOperations>();
    Container.RegisterSingleton<IUserInfoOperations, UserOperations>();
    Container.RegisterSingleton<IUserEditOperations, UserOperations>();

    Container.RegisterSingleton<FacadeMongoDBRequests>();
    Container.Register<IFactory<IMongoClient>, MongoClientFactory>(Lifestyle.Transient);
    Container.RegisterSingleton<IMongoClient>(() => Container.GetInstance<IFactory<IMongoClient>>().Create());
    Container.RegisterSingleton<IMongoDatabase>(() => Container.GetInstance<IMongoClient>().GetDatabase("siren"));
    Container.RegisterSingleton<IMongoCollection<SirenRepresentation>>(()
      => Container.GetInstance<IMongoDatabase>().GetCollection<SirenRepresentation>("sirens"));
    Container.RegisterSingleton<IMongoCollection<UserRepresentation>>(()
      => Container.GetInstance<IMongoDatabase>().GetCollection<UserRepresentation>("users"));
  }
}