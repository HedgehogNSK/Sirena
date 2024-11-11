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
  /// Shortcut for conditional interface registration into target type
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
  static public void RegisterStepFactoryWithBuilderFactories(this Container container, Type stepFactory, Type[] builderFactories)
  {
    var interfaces = stepFactory.GetInterfaces();
    var factories = interfaces.Where(_i => _i.IsGenericType && IsFactoryInterface(_i.GetGenericTypeDefinition()));
    if (!factories.Any())
      throw new TypeInitializationException(stepFactory.FullName, new Exception($"{stepFactory.Name} must implements 1 IFactory interface"));
    var factory = factories.First();
    foreach (var builderFactory in builderFactories)
    {
      interfaces = builderFactory.GetInterfaces();
      var builderInterfaces = interfaces.Where(_i => _i.IsGenericType && IsFactoryInterface(_i.GetGenericTypeDefinition()));
      if (!builderInterfaces.Any())
        throw new TypeInitializationException(builderFactory.FullName, new Exception($"{builderFactory.Name} must implements 1 IFactory interface"));
      var builderInterface = builderInterfaces.First();
      container.RegisterConditional(builderInterface, builderFactory, Lifestyle.Singleton, x => x.Consumer.ImplementationType == stepFactory);
    }
    container.RegisterSingleton(factory, stepFactory);
  }
  static public void RegisterStepFactoryWithBuilderFactory(this Container container, Type stepFactory, Type builderFactory)
  {
    var interfaces = stepFactory.GetInterfaces();
    var factories = interfaces.Where(_i => _i.IsGenericType && IsFactoryInterface(_i.GetGenericTypeDefinition()));
    if (!factories.Any())
      throw new Exception($"{stepFactory.Name} must implements 1 IFactory interface", new TypeInitializationException(stepFactory.FullName, null));
    var factory = factories.First();
    container.RegisterSingleton(factory, stepFactory);

    interfaces = builderFactory.GetInterfaces();
    factories = interfaces.Where(_i => _i.IsGenericType && IsFactoryInterface(_i.GetGenericTypeDefinition()));
    if (!factories.Any())
      throw new Exception($"{builderFactory.Name} must implements 1 IFactory interface", new TypeInitializationException(builderFactory.FullName, null));
    factory = factories.First();
    container.RegisterConditional(factory, builderFactory, Lifestyle.Singleton, x => x.Consumer.ImplementationType == stepFactory);
  }
  static bool IsFactoryInterface(Type genericInterface)
  {
    // Сравниваем generic type с известными вариантами IFactory<>, IFactory<,>, IFactory<,,>, IFactory<,,,>
    return genericInterface == typeof(IFactory<>) ||
           genericInterface == typeof(IFactory<,>) ||
           genericInterface == typeof(IFactory<,,>) ||
           genericInterface == typeof(IFactory<,,,>);
  }
}