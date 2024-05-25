using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace Hedgey.Sirena.Bot;

public class RequestDBToCreateSirenaStep(Container<IRequestContext> contextContainer, CreateSirenaStep.Buffer buffer, ICreateSirenaOperationAsync createSirenAsync)
: CreateSirenaStep(contextContainer, buffer)
{
  private readonly ICreateSirenaOperationAsync createSirenAsync = createSirenAsync;

  public override IObservable<Report> Make()
  {
    var user = buffer.User;
    var sirenaTitle = buffer.SirenaTitle;
    return Observable.Start(() => buffer)
    .SelectMany(_buffer => createSirenAsync.CreateAsync(_buffer.GetUser().UID, _buffer.SirenaTitle).ToObservable())
    .Select(CreateReport);
  }

  private Report CreateReport(SirenRepresentation sirena)
  {
    CommandStep.Result result = sirena!=null? Result.Success : Result.Canceled;
    buffer.MessageBuilder.SetSirena(sirena);
    return new(result, buffer.MessageBuilder);
  }
}