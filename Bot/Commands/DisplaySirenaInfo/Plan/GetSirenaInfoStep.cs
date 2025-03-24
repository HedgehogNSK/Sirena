using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using System.Reactive.Linq;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public class GetSirenaInfoStep(NullableContainer<ulong> sirenaIdContainter
  , IFindSirenaOperation findSirena
  , IFactory<IRequestContext, ulong, ISendMessageBuilder> notFoundMessageBuilderFactory
  , IFactory<IRequestContext, long, SirenRepresentation, SirenaInfoMessageBuilder> sirenaInfoMessageBuilderFactory
  , IGetUserInformation getUserInformation
   ) : CommandStep
{
  public override IObservable<Report> Make(IRequestContext context)
  {
    var uid = context.GetUser().Id;
    var info = context.GetCultureInfo();

    var observableFind = findSirena.Find(sirenaIdContainter.Get()).Publish().RefCount();
    var observableRequestOwnerNickname = observableFind.Where(_siren => _siren != null && _siren.OwnerId != uid)
      .SelectMany(_sirena => getUserInformation.GetNickname(_sirena.OwnerId, info)
            .Do(_nick => _sirena.OwnerNickname = _nick)
            .Select(_ => _sirena));
    return observableFind.Where(_siren => _siren == null || _siren.OwnerId == uid)
    .Merge(observableRequestOwnerNickname).Select(CreateReport);

    Report CreateReport(SirenRepresentation representation)
    {
      long uid = context.GetUser().Id;

      Result result = representation == null ? Result.Canceled : Result.Success;
      ISendMessageBuilder builder = representation == null ?
          notFoundMessageBuilderFactory.Create(context, sirenaIdContainter.Get())
          : sirenaInfoMessageBuilderFactory.Create(context, uid, representation);

      return new Report(result, builder);
    }
  }

  public class Factory(IFindSirenaOperation findSirenaOperation
  , IFactory<IRequestContext, ulong, ISendMessageBuilder> noSirenaMessageBuilder
  , IFactory<IRequestContext, long, SirenRepresentation, SirenaInfoMessageBuilder> sirenaInfoMessageBuilderFactory
  , IGetUserInformation getUserInformation)
    : IFactory<NullableContainer<ulong>, GetSirenaInfoStep>
  {
    public GetSirenaInfoStep Create(NullableContainer<ulong> idContainer)
     => new GetSirenaInfoStep(idContainer, findSirenaOperation, noSirenaMessageBuilder, sirenaInfoMessageBuilderFactory, getUserInformation);
  }
}