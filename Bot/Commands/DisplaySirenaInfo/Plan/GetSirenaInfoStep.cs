using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using MongoDB.Bson;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class GetSirenaInfoStep(NullableContainer<ObjectId> sirenaIdContainter
  , IFindSirenaOperation findSirena
  , IFactory<IRequestContext, ObjectId, SirenaNotFoundMessageBuilder> notFoundMessageBuilderFactory
  , IFactory<IRequestContext, long, SirenRepresentation, SirenaInfoMessageBuilder> sirenaInfoMessageBuilderFactory
   ) : CommandStep
{
  public override IObservable<Report> Make(IRequestContext context)
  {
    return findSirena.Find(sirenaIdContainter.Get()).Select(CreateReport);

    Report CreateReport(SirenRepresentation representation)
    {
      var chatId = context.GetTargetChatId();
      long uid = context.GetUser().Id;
      var info = context.GetCultureInfo();

      Result result = representation == null ? Result.Canceled : Result.Success;
      MessageBuilder builder = representation == null ?
          notFoundMessageBuilderFactory.Create(context, sirenaIdContainter.Get())
          : sirenaInfoMessageBuilderFactory.Create(context, uid, representation);

      return new Report(result, builder);
    }
  }

  public class Factory(IFindSirenaOperation findSirenaOperation
  , IFactory<IRequestContext, ObjectId, SirenaNotFoundMessageBuilder> noSirenaMessageBuilder
  , IFactory<IRequestContext, long, SirenRepresentation, SirenaInfoMessageBuilder> sirenaInfoMessageBuilderFactory)
    : IFactory<NullableContainer<ObjectId>, GetSirenaInfoStep>
  {
    public GetSirenaInfoStep Create(NullableContainer<ObjectId> idContainer)
     => new GetSirenaInfoStep(idContainer, findSirenaOperation, noSirenaMessageBuilder, sirenaInfoMessageBuilderFactory);
  }
}