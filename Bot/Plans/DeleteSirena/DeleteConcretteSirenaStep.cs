using System.Data;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Hedgey.Sirena.Database;

namespace Hedgey.Sirena.Bot;

/*public class FirstRemoveSirenaStep : CommandStep
{
  public FirstRemoveSirenaStep(Container<IRequestContext> contextContainer)
  : base(contextContainer)
  { }

  public override IObservable<Report> Make()
  {
    var context = contextContainer.Object;
    User botUser = context.GetUser();
    long uid = botUser.Id;
    long chatId = context.GetChat().Id;
    string param = context.GetArgsString().GetParameterByNumber(0);
  }
}*/
public class DeleteConcretteSirenaStep : DeleteSirenaStep
{
  private readonly FacadeMongoDBRequests requests;

  public DeleteConcretteSirenaStep(Container<IRequestContext> contextContainer
  , Container<SirenRepresentation> sirenaContainer, FacadeMongoDBRequests requests)
   : base(contextContainer, sirenaContainer)
  {
    this.requests = requests;
  }

  public override IObservable<Report> Make()
  {
    return requests.DeleteUsersSirena(contextContainer.Object.GetTargetChatId()
    , sirenaContainer.Object.Id)
    .ToObservable()
    .Select(CreateReport);
  }

  private Report CreateReport(SirenRepresentation deletedSirena)
  {
    var builder = new SuccesfulDeleteMessageBuilder(contextContainer.Object.GetTargetChatId(), deletedSirena);
    return new Report(Result.Success, builder);
  }
}