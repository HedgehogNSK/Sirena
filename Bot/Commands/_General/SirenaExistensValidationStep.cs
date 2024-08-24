using Hedgey.Localization;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using MongoDB.Bson;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class SirenaExistensValidationStep(NullableContainer<ObjectId> idContainer
, NullableContainer<SirenRepresentation> sirenaContainer
, IFindSirenaOperation findSirenaOperation
, IFactory<IRequestContext, ObjectId, IMessageBuilder> noSirenaMessageBuilderFactory)
 : CommandStep
{
  public override IObservable<Report> Make(IRequestContext context)
  {
    var sirenaId = idContainer.Get();
    return findSirenaOperation.Find(sirenaId).Select(CreateReport);

    Report CreateReport(SirenRepresentation representation)
    {
      var sirenaId = idContainer.Get();
      if (representation == null)
        return new Report(Result.Wait, noSirenaMessageBuilderFactory.Create(context, sirenaId));
      sirenaContainer.Set(representation);
      return new Report(Result.Success);
    }
  }

  public class Factory(IFactory<IRequestContext, ObjectId, IMessageBuilder> messageBuilderFactory
  , IFindSirenaOperation findSirenaOperation)
    : IFactory<NullableContainer<ObjectId>, NullableContainer<SirenRepresentation>, SirenaExistensValidationStep>
  {
    public SirenaExistensValidationStep Create(NullableContainer<ObjectId> idContainer
      , NullableContainer<SirenRepresentation> sirenaContainer)
      => new SirenaExistensValidationStep(idContainer, sirenaContainer
      , findSirenaOperation, messageBuilderFactory);
  }

  public class MessagBuilderFactory(ILocalizationProvider localizationProvider)
    : IFactory<IRequestContext, ObjectId, NoSirenaMessageBuilder>
  {
    public NoSirenaMessageBuilder Create(IRequestContext context, ObjectId sirenaId)
    {
      var chatId = context.GetChat().Id;
      var info = context.GetCultureInfo();
      return new NoSirenaMessageBuilder(chatId, info, localizationProvider, sirenaId);
    }
  }
}