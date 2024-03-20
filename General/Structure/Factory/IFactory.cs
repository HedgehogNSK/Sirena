namespace Hedgey.Structure.Factory;

public interface IFactory<T>{
  T Create();
}

public interface IFactory<TParam, T>{
  T Create(TParam param);
}

public interface IFactory<TParam1,TParam2, T>{
  T Create(TParam1 param1, TParam2 param2);
}