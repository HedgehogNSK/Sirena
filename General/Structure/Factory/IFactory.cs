namespace Hedgey.Structure.Factory;

public interface IFactory<out T>
{
  T Create();
}
public interface IFactory<in T1, out TObject>
{
  TObject Create(T1 param);
}
public interface IFactory<in T1, in T2, out TObject>
{
  TObject Create(T1 param1, T2 param2);
}
public interface IFactory<in T1, in T2, in T3, out TObject>
{
  TObject Create(T1 param1, T2 param2, T3 param3);
}