namespace Hedgey.Sirena;

public class ArgumentNotInitializedException(string paramName) : Exception
{
  public override string Message => $"{base.Message}. \nParameter \"{paramName}\" hasn't been initialized yet";
}
