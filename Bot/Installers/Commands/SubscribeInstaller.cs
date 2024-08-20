namespace Hedgey.Sirena.Bot.DI;

public class SubscribeInstaller(SimpleInjector.Container container)
 : PlanBassedCommandInstaller<SubscribeCommand, SubscribeSirenaPlanFactory>(container)
{ }