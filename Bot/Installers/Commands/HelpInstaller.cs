namespace Hedgey.Sirena.Bot.DI;

public class HelpInstaller(SimpleInjector.Container container)
   : CommandInstaller<HelpCommand>(container)
  { }
