using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Entities;
using Hedgey.Structure.Factory;
using RxTelegram.Bot.Interface.BaseTypes;
using System.Reactive.Linq;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public class SendRequestStep(IRightsRequestOperation request
  , NullableContainer<SirenaData> sirenaContainer
  , IFactory<IRequestContext, IRightsRequestOperation.Result, SirenaData, ISendMessageBuilder> messageBuilderFactory)
  : CommandStep
{
  public override IObservable<Report> Make(IRequestContext context)
  {
    string userMessage = string.Empty;
    var sirena = sirenaContainer.Get();
    long uid = context.GetUser().Id;
    long chatId = context.GetChat().Id;
    Message message = context.GetMessage();
    if (!message.From.IsBot)
      userMessage = context.GetArgsString();
    return request.Send(sirena.SID, uid, userMessage)
      .Select(ProcessResult);

    Report ProcessResult(IRightsRequestOperation.Result requestResult)
    {
      ISendMessageBuilder messageBuilder = messageBuilderFactory.Create(context, requestResult, sirena);
      Result reportResult = requestResult.isSirenaFound && requestResult.isSuccess ?
        Result.Success : Result.Canceled;
      return new Report(reportResult, messageBuilder);
    }
  }

  public class Factory(IFactory<IRequestContext, IRightsRequestOperation.Result, SirenaData, ISendMessageBuilder> messageBuilderFactory
  , IRightsRequestOperation requestRights)
   : IFactory<NullableContainer<SirenaData>, SendRequestStep>
  {

    public SendRequestStep Create(NullableContainer<SirenaData> sirenaContainer)
      => new SendRequestStep(requestRights, sirenaContainer, messageBuilderFactory);
  }
}