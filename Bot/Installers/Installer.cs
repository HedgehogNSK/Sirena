using SimpleInjector;

namespace Hedgey.Sirena.Bot;

public abstract class Installer
{
  public Installer(Container container)
  {
    Container = container;
  }
  public abstract void Install();

  public Container Container { get; }
}