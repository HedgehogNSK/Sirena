using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.MongoDB.Operations;
using Hedgey.Sirena.Entities;
using Hedgey.Structure.Factory;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot.DI
{
  public class CreateSirenaCommandInstaller(SimpleInjector.Container container)
   : PlanBassedCommandInstaller<CreateSirenaCommand, CreateSirenaPlanFactory>(container)
  {
    public override void Install()
    {
      base.Install();

      Container.RegisterSingleton<IGetUserOperationAsync, GetUserOperationAsync>();
      Container.RegisterSingleton<ICreateSirenaOperationAsync, CreateSirenaOperationAsync>();

      Container.RegisterSingleton<IFactory<IRequestContext, CreateMessageBuilder>, CreateMessageBuilder.Factory>();

      Container.Register<NullableContainer<string>>();
      Container.Register<NullableContainer<UserData>>();
    }
  }
}