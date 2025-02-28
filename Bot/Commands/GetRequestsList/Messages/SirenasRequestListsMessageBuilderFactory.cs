using Hedgey.Localization;
using Hedgey.Structure.Factory;

namespace Hedgey.Sirena.Bot;

public class SirenasRequestListsMessageBuilderFactory(ILocalizationProvider localizationProvider)
 : IFactory<IRequestContext, SirenasListMessageBuilder>
{
  private readonly ILocalizationProvider localizationProvider = localizationProvider;

  public SirenasListMessageBuilder Create(IRequestContext context)
  {
    const string introductionKey = "command.requests.list_title";
    const string commandName = RequestsCommand.NAME;
    var chatID = context.GetTargetChatId();
    var info = context.GetCultureInfo();
    return new SirenasListMessageBuilder(chatID, info, localizationProvider
      , introductionKey, commandName);
  }
}