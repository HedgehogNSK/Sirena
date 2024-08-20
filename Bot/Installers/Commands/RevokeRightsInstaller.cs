namespace Hedgey.Sirena.Bot.DI;

public class RevokeRightsInstaller(SimpleInjector.Container container)
   : CommandInstaller<RevokeRightsCommand>(container)
  { }
