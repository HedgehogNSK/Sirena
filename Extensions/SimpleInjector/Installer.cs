using SimpleInjector;

namespace Hedgey.Extensions.SimpleInjector;

public abstract class Installer
{
  protected Installer(Container container)
  {
    Container = container;
  }
  public abstract void Install();

  public Container Container { get; }
}