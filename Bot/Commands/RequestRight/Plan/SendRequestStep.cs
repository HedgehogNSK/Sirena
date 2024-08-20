using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class SendRequestStep : CommandStep
{
  private readonly NullableContainer<string> messageContainer;
  private readonly IRightsRequestOperation request;
  private readonly NullableContainer<SirenRepresentation> sirenaContainer;
  private readonly IFactory<IRightsRequestOperation.Result,IMessageBuilder> messageBuilderFactory;
  public SendRequestStep(IRightsRequestOperation request
  , NullableContainer<SirenRepresentation> sirenaContainer
  ,NullableContainer<string> messageContainer
  ,IFactory<IRightsRequestOperation.Result,IMessageBuilder> messageBuilderFactory)
  {
    this.request = request;
    this.sirenaContainer = sirenaContainer;
    this.messageBuilderFactory = messageBuilderFactory;
    this.messageContainer = messageContainer;
  }

  public override IObservable<Report> Make(IRequestContext context)
  {
    string userMessage = messageContainer.Get();
    MongoDB.Bson.ObjectId sid = sirenaContainer.Get().Id;
    long uid = context.GetUser().Id;
    long chatId = context.GetChat().Id;
    return request.Send(sid, uid, userMessage)
      .Select(ProcessResult);
  }

  private Report ProcessResult(IRightsRequestOperation.Result requestResult)
  {
    var messageBuilder = messageBuilderFactory.Create(requestResult);
    var reportResult = requestResult.isSirenaFound && requestResult.isSuccess?
      Result.Success : Result.Canceled;
    return new Report(reportResult, messageBuilder);
  }
}