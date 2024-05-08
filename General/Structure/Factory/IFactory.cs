namespace Hedgey.Structure.Factory;

public interface IFactory<T>
{
  T Create();
}
public interface IFactory<T1, TObject>
{
  TObject Create(T1 param);
}
public interface IFactory<T1, T2, TObject>
{
  TObject Create(T1 param1, T2 param2);
}
public interface IFactory<T1, T2, T3, TObject>
{
  TObject Create(T1 param1, T2 param2, T3 param3);
}