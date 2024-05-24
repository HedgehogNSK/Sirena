namespace Hedgey.Sirena.Bot;

public class NullableContainer<T>
{
  public T? Content { get; private set;}
  public NullableContainer(){}
  public NullableContainer(T obj) { Content = obj; }
  public virtual void Set(T newObject)
  {
    Content = newObject;
  }

  internal T Get()
  {
    return Content??
       throw new ArgumentNullException("Value were not initialized yet",typeof(T?).Name);
  }
}