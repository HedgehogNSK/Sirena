using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Entities;
using Hedgey.Structure.Factory;
using System.Data;
using System.Reactive.Linq;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public class DeleteConcretteSirenaStep : DeleteSirenaStep
{
  public class Factory(IFactory<IRequestContext, SirenaData, SuccesfulDeleteMessageBuilder> messageBuilderFactory
  , IDeleteSirenaOperation sirenaOperations)
    : IFactory<NullableContainer<SirenaData>, DeleteConcretteSirenaStep>
  {
    public DeleteConcretteSirenaStep Create(NullableContainer<SirenaData> sirenaContainer)
      => new DeleteConcretteSirenaStep(sirenaContainer, sirenaOperations, messageBuilderFactory);
  }
  private readonly IDeleteSirenaOperation sirenaDeleteOperation;
  private readonly IFactory<IRequestContext, SirenaData, SuccesfulDeleteMessageBuilder> messageBuilderFactory;

  public DeleteConcretteSirenaStep(NullableContainer<SirenaData> sirenaContainer
  , IDeleteSirenaOperation sirenaDeleteOperation
  , IFactory<IRequestContext, SirenaData, SuccesfulDeleteMessageBuilder> messageBuilderFactory)
    : base(sirenaContainer)
  {
    this.sirenaDeleteOperation = sirenaDeleteOperation;
    this.messageBuilderFactory = messageBuilderFactory;
  }

  public override IObservable<Report> Make(IRequestContext context)
  {
    var uid = context.GetUser().Id;
    var sirenaId = sirenaContainer.Get().SID;
    return sirenaDeleteOperation.Delete(uid, sirenaId)
    .Select(CreateReport);

    Report CreateReport(SirenaData deletedSirena)
    {
      var info = context.GetCultureInfo();
      var builder = messageBuilderFactory.Create(context, deletedSirena);
      return new Report(Result.Success, builder);
    }
  }
}