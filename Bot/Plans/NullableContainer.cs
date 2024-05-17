namespace Hedgey.Sirena.Bot;

public class NullableContainer<T>
{
  public T? Object { get; private set;}
  public NullableContainer(){}
  public NullableContainer(T obj) { Object = obj; }
  public virtual void Set(T newObject)
  {
    Object = newObject;
  }

  internal T Get()
  {
    return Object??
       throw new ArgumentNullException("Value were not initialized yet",typeof(T?).Name);
  }
}