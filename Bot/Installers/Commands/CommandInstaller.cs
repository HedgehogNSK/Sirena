using Hedgey.Extensions.SimpleInjector;
using SimpleInjector;

namespace Hedgey.Sirena.Bot.DI;

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
  protected void RegisterStepFactoryIntoCommand<TStepInterface>(
    Lifestyle lifestyle
    , Func<TStepInterface> instanceCreator)
  where TStepInterface : class
  {
    var registration = lifestyle.CreateRegistration(instanceCreator, Container);
    Container.RegisterConditional<TStepInterface>(registration
      , (_context) => _context.Consumer.ImplementationType == typeof(T));
  }
}