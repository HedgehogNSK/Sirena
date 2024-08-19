using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class SirenaStateValidationStep(NullableContainer<SirenRepresentation> sirenaContainer
  , IFactory<IRequestContext, SirenRepresentation, IMessageBuilder> messageBuilderFactory)
  : CommandStep
{
  static public readonly TimeSpan allowedCallPeriod = TimeSpan.FromMinutes(1);

  public override IObservable<Report> Make(IRequestContext context)
  {
    long uid = context.GetUser().Id;

    SirenRepresentation sirena = sirenaContainer.Get();
    Report report;
    if (sirena.CanBeCalledBy(uid) && IsReadyToCall(sirena))
    {
      report = new Report(Result.Success, null);
    }
    else
      report = new Report(Result.Canceled, messageBuilderFactory.Create(context, sirena));
    return Observable.Return(report);
  }

  private static bool IsReadyToCall(SirenRepresentation sirena)
  {
    if (sirena.LastCall == null)
      return true;

    var timePassed = DateTimeOffset.UtcNow - sirena.LastCall.Date;
    return timePassed > allowedCallPeriod;
  }
  public class Factory(IFactory<IRequestContext, SirenRepresentation, IMessageBuilder> messageBuilderFactory)
     : IFactory<NullableContainer<SirenRepresentation>, SirenaStateValidationStep>
  {
    public SirenaStateValidationStep Create(NullableContainer<SirenRepresentation> sirenaContainer)
      => new SirenaStateValidationStep(sirenaContainer, messageBuilderFactory);
  }
}