namespace Hedgey.Sirena.Bot.DI;

public class DeleteSirenaInstaller(SimpleInjector.Container container)
 : PlanBassedCommandInstaller<DeleteSirenaCommand, DeleteSirenaPlanFactory>(container)
  { }