using Hedgey.Extensions;
using Hedgey.Localization;
using Hedgey.Structure.Factory;
using MongoDB.Bson;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class SirenaIdValidationStep : CommandStep
{
  private readonly NullableContainer<ObjectId> idContainer;
  private readonly IFactory<IRequestContext, string, IMessageBuilder> messageBuilderFactory;
  private readonly int idArgNumber;

  public SirenaIdValidationStep(IFactory<IRequestContext, string, IMessageBuilder> messageBuilderFactory
    , NullableContainer<ObjectId> idContainer
    , int idArgNumber = 0)
  {
    this.idContainer = idContainer;
    this.messageBuilderFactory = messageBuilderFactory;
    this.idArgNumber = idArgNumber;
  }

  public override IObservable<Report> Make(IRequestContext context)
  {
    var param = context.GetArgsString().GetParameterByNumber(idArgNumber);

    Report report;
    if (!ObjectId.TryParse(param, out ObjectId sirenaId))
    {
      var builder = messageBuilderFactory.Create(context, param);
      report = new Report(Result.Wait, builder);
    }
    else
    {
      idContainer.Set(sirenaId);
      report = new Report(Result.Success);
    }
    return Observable.Return(report);
  }
  public class Factory(IFactory<IRequestContext, string, IMessageBuilder> messageBuilderFactory)
    : IFactory<NullableContainer<ObjectId>, SirenaIdValidationStep>
  {
    public SirenaIdValidationStep Create(NullableContainer<ObjectId> idContainer)
      => new SirenaIdValidationStep(messageBuilderFactory, idContainer, 0);
  }
  public class MessageBuilderFactory(ILocalizationProvider localizationProvider)
    : IFactory<IRequestContext, string, IMessageBuilder>
  {
    public IMessageBuilder Create(IRequestContext context, string param)
    {
      var chatId = context.GetChat().Id;
      var info = context.GetCultureInfo();
      return new StringNotIdMessageBuilder(chatId, info, localizationProvider, param);
    }
  }
}