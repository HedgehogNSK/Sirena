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
    container.RegisterConditional<TInterface, TImpl>(Lifestyle.Singleton, _context => _context.Consumer.ImplementationType == typeof(TTarget));
  }
  static public void RegisterStepFactoryWithBuilderFactories(this Container container, Type consumerFactory, Type[] providerFactories)
  {
    var interfaces = consumerFactory.GetInterfaces();
    var factories = interfaces.Where(_i => _i.IsGenericType && IsFactoryInterface(_i.GetGenericTypeDefinition()));
    if (!factories.Any())
      throw new TypeInitializationException(consumerFactory.FullName, new Exception($"{consumerFactory.Name} must implements 1 IFactory interface"));
    var factory = factories.First();
    foreach (var builderFactory in providerFactories)
    {
      interfaces = builderFactory.GetInterfaces();
      var builderInterfaces = interfaces.Where(_i => _i.IsGenericType && IsFactoryInterface(_i.GetGenericTypeDefinition()));
      if (!builderInterfaces.Any())
        throw new TypeInitializationException(builderFactory.FullName, new Exception($"{builderFactory.Name} must implements 1 IFactory interface"));
      foreach (var builderInterface in builderInterfaces)
        container.RegisterConditional(builderInterface, builderFactory, Lifestyle.Singleton, x => x.Consumer.ImplementationType == consumerFactory);
    }
    container.RegisterSingleton(factory, consumerFactory);
  }
  static public void RegisterStepFactoryWithBuilderFactory(this Container container, Type consumerFactory, Type providerFactory)
  {
    var interfaces = consumerFactory.GetInterfaces();
    var factories = interfaces.Where(_i => _i.IsGenericType && IsFactoryInterface(_i.GetGenericTypeDefinition()));
    if (!factories.Any())
      throw new Exception($"{consumerFactory.Name} must implements 1 IFactory interface", new TypeInitializationException(consumerFactory.FullName, null));
    var factory = factories.First();
    container.RegisterSingleton(factory, consumerFactory);

    interfaces = providerFactory.GetInterfaces();
    factories = interfaces.Where(_i => _i.IsGenericType && IsFactoryInterface(_i.GetGenericTypeDefinition()));
    if (!factories.Any())
      throw new Exception($"{providerFactory.Name} must implements 1 IFactory interface", new TypeInitializationException(providerFactory.FullName, null));
    factory = factories.First();
    container.RegisterConditional(factory, providerFactory, Lifestyle.Singleton, x => x.Consumer.ImplementationType == consumerFactory);
  }
  static bool IsFactoryInterface(Type genericInterface)
  {
    return genericInterface == typeof(IFactory<>) ||
           genericInterface == typeof(IFactory<,>) ||
           genericInterface == typeof(IFactory<,,>) ||
           genericInterface == typeof(IFactory<,,,>) ||
           genericInterface == typeof(IFactory<,,,,>);
  }
}