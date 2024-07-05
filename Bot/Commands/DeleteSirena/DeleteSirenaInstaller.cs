using SimpleInjector;

namespace Hedgey.Sirena.Bot;

public class DeleteSirenaInstaller(Container container)
:PlanBassedCommandInstaller<DeleteSirenaCommand,DeleteSirenaPlanFactory>(container)
{}
