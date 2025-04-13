using Hedgey.Extensions.SimpleInjector;
using SimpleInjector;

namespace Hedgey.Sirena.Bot.DI;

public class CommandInstaller<T>(Container container) : Installer(container)
 where T : AbstractBotCommmand
{
  protected Lifestyle lifestyle = Lifestyle.Singleton;
  public override void Install()
  {
    Container.Register<T>(lifestyle);
    Container.RegisterInitializer<T>(_command =>
    {
      var dict = Container.GetInstance<ActiveCommandsDictionary>();
      dict[_command.Command] = _command;
    });
  }
  protected void RegisterIntoCommand<TInterface>(
    Lifestyle lifestyle
    , Func<TInterface> instanceCreator)
  where TInterface : class
  {
    var registration = lifestyle.CreateRegistration(instanceCreator, Container);
    Container.RegisterConditional<TInterface>(registration
      , (_context) => _context.Consumer.ImplementationType == typeof(T));
  }
  protected void RegisterIntoCommand<TInterface, TImpl>(
    Lifestyle lifestyle)
  where TImpl : class, TInterface
  {
    var registration = lifestyle.CreateRegistration<TImpl>(Container);
    Container.RegisterConditional<TInterface>(registration
      , (_context) => _context.Consumer.ImplementationType == typeof(T));
  }
  protected void RegisterIntoCommand<TImpl>(Lifestyle lifestyle)
  where TImpl : class
    =>Container.RegisterConditional<TImpl,TImpl>(lifestyle,
      (_context) => _context.Consumer.ImplementationType == typeof(T));
}