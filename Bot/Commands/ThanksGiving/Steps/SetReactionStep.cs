using Hedgey.Extensions;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Structure.Factory;
using Hedgey.Telegram.Bot;
using MongoDB.Bson;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class SetReactionStep(ISirenaActivationOperation activationOperation
  , NullableContainer<ObjectId> idContainer)
: CommandStep
{
  public override IObservable<Report> Make(IRequestContext context)
  {
    var id = idContainer.Get();
    var uid = context.GetUser().Id;
    var emojiString = context.GetArgsString().GetParameterByNumber(1);
    var emoji = int.Parse(emojiString);
    return activationOperation.SetReaction(id, uid, emoji)
      .Select(_call =>
      {
        if (_call == null)
          return new Report(Result.Canceled);
        var editBuilder = new ReactionEditMessageReplyMarkupBuilder(context);
        return new Report(Result.Success, EditMessageReplyMarkupBuilder: editBuilder);
      });
  }
  public class Factory(ISirenaActivationOperation activationOperation)
  : IFactory<NullableContainer<ObjectId>, SetReactionStep>
  {
    public SetReactionStep Create(NullableContainer<ObjectId> idContainer)
      => new SetReactionStep(activationOperation, idContainer);
  }
}