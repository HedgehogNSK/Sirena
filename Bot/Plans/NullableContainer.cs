namespace Hedgey.Sirena.Bot;

public class NullableContainer<T>
{
  public T? Content { get; private set;}
  public NullableContainer(){}
  public virtual void Set(T newObject)
  {
    Content = newObject;
  }
  public bool IsSet => !EqualityComparer<T?>.Default.Equals(Content, default);
  public T Get()
  {
    return Content??
       throw new ArgumentNullException(typeof(T?).Name,"Value were not initialized yet");
  }
}