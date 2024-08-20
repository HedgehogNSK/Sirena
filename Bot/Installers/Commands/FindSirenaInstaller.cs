using SimpleInjector;

namespace Hedgey.Sirena.Bot.DI;

public class FindSirenaInstaller(Container container)
 : PlanBassedCommandInstaller<FindSirenaCommand,FindSirenaPlanFactory>(container)
{}