using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Bot.Operations.Mongo;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;

namespace Hedgey.Sirena.Bot.DI
{
  public class CreateSirenaCommandInstaller(SimpleInjector.Container container)
   : PlanBassedCommandInstaller<CreateSirenaCommand, CreateSirenaPlanFactory>(container)
  {
    public override void Install()
    {
      base.Install();

      Container.Register<IGetUserOperationAsync, GetUserOperationAsync>();
      Container.Register<ICreateSirenaOperationAsync, CreateSirenaOperationAsync>();

      Container.RegisterSingleton<IFactory<IRequestContext, CreateMessageBuilder>, CreateMessageBuilder.Factory>();

      Container.Register<NullableContainer<string>>();
      Container.Register<NullableContainer<UserRepresentation>>();
    }
  }
}