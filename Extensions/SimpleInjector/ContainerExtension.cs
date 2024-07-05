using Hedgey.Structure.Factory;
using SimpleInjector;

namespace Hedgey.Extensions.SimpleInjector;

static public class ContainerExtension{
    static public TResult GetInstanceFrom<TParam, TResult>(this Container container, TParam param)
    => container.GetInstance<IFactory<TParam, TResult>>().Create(param);
  static public TResult GetInstanceFrom<T1, T2, TResult>(this Container container, T1 param1, T2 param2)
    => container.GetInstance<IFactory<T1, T2, TResult>>().Create(param1, param2);
  static public TResult GetInstanceFrom<T1, T2, T3, TResult>(this Container container, T1 param1, T2 param2, T3 param3)
    => container.GetInstance<IFactory<T1, T2, T3, TResult>>().Create(param1, param2, param3);

}