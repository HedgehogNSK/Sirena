namespace Hedgey.Sirena.Bot.DI;

public class UnmuteUserSignalInstaller(SimpleInjector.Container container)
   : CommandInstaller<UnmuteUserSignalCommand>(container)
{ }