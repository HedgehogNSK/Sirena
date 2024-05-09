namespace Hedgey.Sirena.Bot;

/// <summary>
/// Simple container that stores value of T
/// </summary>
/// <typeparam name="T"></typeparam>
public class Container<T>
{
  public T? Object { get; private set;}
  public Container(){}
  public Container(T obj) { Object = obj; }
  public virtual void Set(T newObject)
  {
    Object = newObject;
  }
}