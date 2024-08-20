namespace Hedgey.Sirena.Bot.DI;

public class MenuBotInstaller(SimpleInjector.Container container)
   : CommandInstaller<MenuBotCommand>(container)
  { }
