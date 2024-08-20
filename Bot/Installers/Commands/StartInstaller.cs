namespace Hedgey.Sirena.Bot.DI;

public class StartInstaller(SimpleInjector.Container container)
   : CommandInstaller<StartCommand>(container)
{ }
