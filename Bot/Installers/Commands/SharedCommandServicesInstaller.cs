using Hedgey.Extensions.SimpleInjector;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.MongoDB.Operations;
using Hedgey.Sirena.Entities;
using Hedgey.Structure.Factory;
using MongoDB.Driver;
using SimpleInjector;

namespace Hedgey.Sirena.MongoDB.DI;

public class SharedCommandServicesInstaller(Container container) : Installer(container)
{
  public override void Install()
  {
    Container.RegisterSingleton<IDeleteSirenaOperation, SirenaOperations>();
    Container.RegisterSingleton<IFindSirenaOperation, SirenaOperations>();
    Container.RegisterSingleton<IGetUserRelatedSirenas, SirenaOperations>();
    Container.RegisterSingleton<ISubscribeToSirenaOperation, SirenaOperations>();
    Container.RegisterSingleton<IRightsManageOperation, SirenaOperations>();
    Container.RegisterSingleton<IUpdateSirenaOperation, SirenaOperations>();
    Container.RegisterSingleton<IUnsubscribeSirenaOperation, SirenaOperations>();
    Container.RegisterSingleton<ISirenaActivationOperation, SirenaOperations>();
    Container.RegisterSingleton<IGetUserInformation, GetUserInformation>();
    Container.RegisterSingleton<IGetUserOverviewAsync, UserOperations>();
    Container.RegisterSingleton<IUserInfoOperations, UserOperations>();
    Container.RegisterSingleton<IUserEditOperations, UserOperations>();

    Container.RegisterSingleton<FacadeMongoDBRequests>();
    Container.Register<IFactory<IMongoClient>, MongoClientFactory>(Lifestyle.Transient);
    Container.RegisterSingleton<IMongoClient>(() => Container.GetInstance<IFactory<IMongoClient>>().Create());
    Container.RegisterSingleton<IMongoDatabase>(() => Container.GetInstance<IMongoClient>().GetDatabase("siren"));
    Container.RegisterSingleton<IMongoCollection<SirenaData>>(()
      => Container.GetInstance<IMongoDatabase>().GetCollection<SirenaData>("sirens"));
    Container.RegisterSingleton<IMongoCollection<UserData>>(()
      => Container.GetInstance<IMongoDatabase>().GetCollection<UserData>("users"));
    Container.RegisterSingleton<IMongoCollection<SirenaActivation>>(()
      => Container.GetInstance<IMongoDatabase>().GetCollection<SirenaActivation>("calls"));
  }
}