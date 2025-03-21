using Hedgey.Blendflake;
using Hedgey.Extensions;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using System.Reactive.Linq;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public class CheckCallAbilityStep(
  IFactory<IRequestContext, IEnumerable<SirenRepresentation>, ISendMessageBuilder> availableSirenasMessageBuilderFactory
, IGetUserRelatedSirenas getUserRelatedSirenas
, NullableContainer<ulong> idContainer
, int idArgNumber = 0) : CommandStep
{
  public override IObservable<Report> Make(IRequestContext context)
  {
    var param = context.GetArgsString().GetParameterByNumber(idArgNumber);

    if (!HashUtilities.TryParse(param, out var sirenaId))
    {
      var uid = context.GetUser().Id;
      return getUserRelatedSirenas.GetAvailableForCallSirenas(uid).Select(CreateReport);
    }

    idContainer.Set(sirenaId);
    Report report = new Report(Result.Success);
    return Observable.Return(report);

    Report CreateReport(IEnumerable<SirenRepresentation> sirenas)
    {
      ISendMessageBuilder builder = availableSirenasMessageBuilderFactory.Create(context, sirenas);
      return new Report(Result.Canceled, builder);
    };
  }
  public class Factory(IFactory<IRequestContext, IEnumerable<SirenRepresentation>, ISendMessageBuilder> availableSirenasMessageBuilderFactory
  , IGetUserRelatedSirenas getUserRelatedSirenas)
    : IFactory<NullableContainer<ulong>, CheckCallAbilityStep>
  {
    public CheckCallAbilityStep Create(NullableContainer<ulong> idContainer)
      => new CheckCallAbilityStep(availableSirenasMessageBuilderFactory, getUserRelatedSirenas, idContainer, 0);
  }
}