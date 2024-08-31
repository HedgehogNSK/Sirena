namespace Hedgey.Structure;

public interface IGetter<out T> {
  T Get();
}
