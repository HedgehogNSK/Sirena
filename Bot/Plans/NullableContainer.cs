namespace Hedgey.Sirena.Bot;

public class NullableContainer<T>
{
  public T? Content { get; private set;}
  public NullableContainer(){}
  public virtual void Set(T newObject)
  {
    Content = newObject;
  }
  public bool IsSet() => Content != null;
  public T Get()
  {
    return Content??
       throw new ArgumentNullException("Value were not initialized yet",typeof(T?).Name);
  }
  public bool IsEmpty => Content == null;
}