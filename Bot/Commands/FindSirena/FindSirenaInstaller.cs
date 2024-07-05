using SimpleInjector;

namespace Hedgey.Sirena.Bot;

public class FindSirenaInstaller(Container container)
 : PlanBassedCommandInstaller<FindSirenaCommand,FindSirenaPlanFactory>(container)
{}
