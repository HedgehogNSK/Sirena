using Hedgey.Localization;
using Hedgey.Sirena.Entities;
using Hedgey.Structure.Factory;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public class SubscriptionsMessageBuilderFactory(ILocalizationProvider localizationProvider)
   : IFactory<IRequestContext, IEnumerable<SirenaData>, ISendMessageBuilder>
{
  public ISendMessageBuilder Create(IRequestContext context, IEnumerable<SirenaData> source)
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