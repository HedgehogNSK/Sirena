namespace Hedgey.Sirena.Bot.DI;

public class UnsubscribeInstaller(SimpleInjector.Container container)
 : PlanBassedCommandInstaller<UnsubscribeCommand, UnsubscribeSirenaPlanFactory>(container)
{ }