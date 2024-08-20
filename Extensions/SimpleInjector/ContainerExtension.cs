using Hedgey.Structure.Factory;
using SimpleInjector;

namespace Hedgey.Extensions.SimpleInjector;

static public class ContainerExtension
{
  static public TResult GetInstanceFrom<TParam, TResult>(this Container container, TParam param)
  => container.GetInstance<IFactory<TParam, TResult>>().Create(param);
  static public TResult GetInstanceFrom<T1, T2, TResult>(this Container container, T1 param1, T2 param2)
    => container.GetInstance<IFactory<T1, T2, TResult>>().Create(param1, param2);
  static public TResult GetInstanceFrom<T1, T2, T3, TResult>(this Container container, T1 param1, T2 param2, T3 param3)
    => container.GetInstance<IFactory<T1, T2, T3, TResult>>().Create(param1, param2, param3);

  /// <summary>
  /// Shortcut for conditional registration of interface target type
  /// </summary>
  /// <typeparam name="TInterface">Interface of entity</typeparam>
  /// <typeparam name="TImpl">exact implementation of the interface that we are going to inject inside target class</typeparam>
  /// <typeparam name="TTarget">target class</typeparam>
  /// <param name="container"></param>
  static public void RegisterConditional<TInterface, TImpl, TTarget>(this Container container)
    where TInterface : class
    where TImpl : class, TInterface
  {
    container.RegisterConditional<TInterface, TImpl>(Lifestyle.Singleton, c => c.Consumer.ImplementationType == typeof(TTarget));
  }
}