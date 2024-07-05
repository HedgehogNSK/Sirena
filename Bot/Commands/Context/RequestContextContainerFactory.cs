using Hedgey.Structure.Factory;

namespace Hedgey.Sirena.Bot;

public class RequestContextContainerFactory : IFactory<IRequestContext,  Container<IRequestContext>>
{
 public Container<IRequestContext> Create(IRequestContext param)
     => new Container<IRequestContext>(param);
}