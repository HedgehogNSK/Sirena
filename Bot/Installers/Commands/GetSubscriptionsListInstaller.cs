namespace Hedgey.Sirena.Bot.DI;

public class GetSubscriptionsListInstaller(SimpleInjector.Container container)
   : CommandInstaller<GetSubscriptionsListCommand>(container)
  {
    public override void Install()
    {
      base.Install();
    }
  }
