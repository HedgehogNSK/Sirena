namespace Hedgey.Sirena.Bot.DI;

public class DisplaySirenaInfoInstaller(SimpleInjector.Container container)
 : PlanBassedCommandInstaller<DisplaySirenaInfoCommand, DisplaySirenaInfoPlanFactory>(container)
  { }