namespace Hedgey.Sirena.Bot.DI;

public class MuteUserSignalInstaller(SimpleInjector.Container container)
   : CommandInstaller<MuteUserSignalCommand>(container)
  { }
