using SimpleInjector;

namespace Hedgey.Sirena.Bot;

public class CommandInstaller<T>(Container container) : Installer(container)
 where T : AbstractBotCommmand
{
  public override void Install()
  {
    Container.Register<T>();
    Container.RegisterInitializer<T>(_command =>
    {
      var dict = Container.GetInstance<ActiveCommandsDictionary>();
      dict[_command.Command] = _command;
    });
  }
}
