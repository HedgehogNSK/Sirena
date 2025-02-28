using Hedgey.Blendflake;
using Hedgey.Localization;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using Hedgey.Utilities;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class SirenaExistensValidationStep(NullableContainer<ulong> idContainer
, NullableContainer<SirenRepresentation> sirenaContainer
, IFindSirenaOperation findSirenaOperation
, IFactory<IRequestContext, string, ISendMessageBuilder> noSirenaMessageBuilderFactory)
 : CommandStep
{
  public override IObservable<Report> Make(IRequestContext context)
  {
    var sirenaId = idContainer.Get();
    return findSirenaOperation.Find(sirenaId).Select(CreateReport);

    Report CreateReport(SirenRepresentation representation)
    {
      string sid = NotBase64URL.From(sirenaId);
      sid = HashUtilities.Shortify(sid);
      if (representation == null)
        return new Report(Result.Wait, noSirenaMessageBuilderFactory.Create(context, sid));
      sirenaContainer.Set(representation);
      return new Report(Result.Success);
    }
  }

  public class Factory(IFactory<IRequestContext, string, ISendMessageBuilder> messageBuilderFactory
  , IFindSirenaOperation findSirenaOperation)
    : IFactory<NullableContainer<ulong>, NullableContainer<SirenRepresentation>, SirenaExistensValidationStep>
  {
    public SirenaExistensValidationStep Create(NullableContainer<ulong> idContainer
      , NullableContainer<SirenRepresentation> sirenaContainer)
      => new SirenaExistensValidationStep(idContainer, sirenaContainer
      , findSirenaOperation, messageBuilderFactory);
  }

  public class MessagBuilderFactory(ILocalizationProvider localizationProvider)
    : IFactory<IRequestContext, string, NoSirenaMessageBuilder>
  {
    public NoSirenaMessageBuilder Create(IRequestContext context, string sirenaId)
    {
      var chatId = context.GetChat().Id;
      var info = context.GetCultureInfo();
      return new NoSirenaMessageBuilder(chatId, info, localizationProvider, sirenaId);
    }
  }
}