using Hedgey.Sirena.Bot.Operations;
using Hedgey.Structure.Factory;
using System.Reactive.Linq;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public class CreateUserStep(IUserEditOperations userEdit
  , IFactory<IRequestContext, ISendMessageBuilder> messageBuilderFactory
  , NullableContainer<UpdateState> updateState)
  : CommandStep
{
  private readonly IUserEditOperations userEdit = userEdit;
  private readonly IFactory<IRequestContext, ISendMessageBuilder> messageBuilderFactory = messageBuilderFactory;
  private readonly NullableContainer<UpdateState> updateState = updateState;

  public override IObservable<Report> Make(IRequestContext context)
  {
    var uid = context.GetUser().Id;
    var cid = context.GetChat().Id;

    return userEdit.CreateUser(uid, cid).Select(ProcessUserCreation);

    Report ProcessUserCreation(UpdateState state)
    {
      updateState.Set(state);
      //If the server responds, then the following statement is true,
      // regardless of whether the user already exists or has been created.
      if ((state | UpdateState.Success) == state)
        return new Report(Result.Success, null);

      var builder = messageBuilderFactory.Create(context);
      return new Report(Result.Canceled, builder);
    }
  }

  public class Factory(IUserEditOperations userEdit
  , IFactory<IRequestContext, ISendMessageBuilder> messageBuilderFactory)
    : IFactory<NullableContainer<UpdateState>, CreateUserStep>
  {
    private readonly IUserEditOperations userEdit = userEdit;
    private readonly IFactory<IRequestContext, ISendMessageBuilder> messageBuilderFactory = messageBuilderFactory;

    public CreateUserStep Create(NullableContainer<UpdateState> updateState)
      => new(userEdit, messageBuilderFactory, updateState);
  }
}