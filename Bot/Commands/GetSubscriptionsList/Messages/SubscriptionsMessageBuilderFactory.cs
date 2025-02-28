using Hedgey.Localization;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;

namespace Hedgey.Sirena.Bot;

public class SubscriptionsMessageBuilderFactory(ILocalizationProvider localizationProvider)
   : IFactory<IRequestContext, IEnumerable<SirenRepresentation>, ISendMessageBuilder>
{
  public ISendMessageBuilder Create(IRequestContext context, IEnumerable<SirenRepresentation> source)
  {
    var chatId = context.GetTargetChatId();
    var userId = context.GetUser().Id;
    var info = context.GetCultureInfo();
    const string prefix = "command.subscriptions.";
    IMessageStrategy headerKey = new MessageStrategy(localizationProvider, prefix + "header");
    IMessageStrategy descriptionKey = new MessageStrategy(localizationProvider, prefix + "bref_info");
    IMessageStrategy emptyListKey = new MessageStrategy(localizationProvider, prefix + "noSubscriptions");
    return new SirenasListMesssageBuilder(chatId, info, localizationProvider, userId
      , source, headerKey, descriptionKey, emptyListKey);
  }
}