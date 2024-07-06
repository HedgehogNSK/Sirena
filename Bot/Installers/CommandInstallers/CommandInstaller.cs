using SimpleInjector;

namespace Hedgey.Sirena.Bot;

public class CommandInstaller<T>(Container container) : Installer(container)
 where T : AbstractBotCommmand
{
  protected Lifestyle lifestyle = Lifestyle.Transient;
  public override void Install()
  {
    Container.Register<T>(lifestyle);
    Container.RegisterInitializer<T>(_command =>
    {
      var dict = Container.GetInstance<ActiveCommandsDictionary>();
      dict[_command.Command] = _command;
    });
  }
}
