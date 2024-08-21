using Hedgey.Structure.Factory;
using MongoDB.Bson;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class ValidateSirenaIdStep(NullableContainer<ObjectId> sirenaIdContainter
  , IFactory<IRequestContext, IMessageBuilder> messageBuilderFactory) : CommandStep
{
  public override IObservable<Report> Make(IRequestContext context)
  {
    var key = context.GetArgsString();

    if (string.IsNullOrEmpty(key) || !ObjectId.TryParse(key, out var id))
      return Observable.Return(new Report(Result.Wait, messageBuilderFactory.Create(context)));

    sirenaIdContainter.Set(id);
    return Observable.Return(new Report(Result.Success));
  }

  public class Factory(IFactory<IRequestContext, IMessageBuilder> messageBuilderFactory)
    : IFactory<NullableContainer<ObjectId>, ValidateSirenaIdStep>
  {
    public ValidateSirenaIdStep Create(NullableContainer<ObjectId> sirenaContainer)
      => new ValidateSirenaIdStep(sirenaContainer, messageBuilderFactory);
  }
}