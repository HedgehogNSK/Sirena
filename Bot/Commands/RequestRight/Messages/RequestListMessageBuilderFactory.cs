using Hedgey.Localization;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;

namespace Hedgey.Sirena.Bot;

public class RequestListMessageBuilderFactory(ILocalizationProvider localizationProvider)
   : IFactory<IRequestContext, IEnumerable<SirenRepresentation>, IMessageBuilder>
  {
    public IMessageBuilder Create(IRequestContext context, IEnumerable<SirenRepresentation> source)
    {
      var chatId = context.GetTargetChatId();
      var info = context.GetCultureInfo();
      const string prefix = "command.request_rights.available.";
      IMessageStrategy headerKey = new MessageStrategy(localizationProvider, prefix + "header");
      IMessageStrategy descriptionKey = new MessageStrategy(localizationProvider, "command.subscriptions.bref_info");
      IMessageStrategy emptyListKey = new MessageStrategy(localizationProvider, prefix + "empty");
      return new SirenasListMesssageBuilder(chatId, info, localizationProvider
        , source,headerKey,descriptionKey,emptyListKey);
    }
  }
