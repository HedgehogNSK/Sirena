using Hedgey.Extensions;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using MongoDB.Bson;
using RxTelegram.Bot.Interface.BaseTypes;
using System.Data;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class FindRemoveSirenaStep : DeleteSirenaStep
{
  private readonly IFindSirenaOperation findSirenaOperation;
  private readonly IFindUserSirenasOperation findUsersSirenaOperation;

  public FindRemoveSirenaStep(Container<IRequestContext> contextContainer
  , Container<SirenRepresentation> sirenaContainer
  , IFindSirenaOperation findSirenaOperation
  , IFindUserSirenasOperation findUsersSirenaOperation)
  : base(contextContainer, sirenaContainer)
  {
    this.findSirenaOperation = findSirenaOperation;
    this.findUsersSirenaOperation = findUsersSirenaOperation;
  }

  public override IObservable<Report> Make()
  {
    var context = contextContainer.Object;
    User botUser = context.GetUser();
    long uid = botUser.Id;
    long chatId = context.GetTargetChatId();
    string param = context.GetArgsString().GetParameterByNumber(0);
    IObservable<SirenRepresentation?> observableSirena;
    int number = 0;
    if (string.IsNullOrEmpty(param))
    {
      return findUsersSirenaOperation.Find(uid)
        .Select(_sireans => new RemoveSirenaMenuMessageBuilder(chatId,_sireans))
        .Select(_removeMenuBuilder => new Report(Result.CanBeFixed, _removeMenuBuilder));
    }
    else if (ObjectId.TryParse(param, out var id))
    {
      observableSirena = findSirenaOperation.Find(id);
    }
    else if (int.TryParse(param, out number))
    {
      observableSirena = findUsersSirenaOperation.FindBySerialNumber(uid, number);
    }
    else
    {
      var builder = new IncorrectParameterMessageBuilder(chatId);
      var report = new Report(Result.CanBeFixed, builder);
      return Observable.Return(report);
    }

    return observableSirena.Select((_sirena) => ProcessRequestById(_sirena, context));
  }

  private Report ProcessRequestById(SirenRepresentation? sirena, IRequestContext context)
  {
    if (sirena == null || sirena.OwnerId != context.GetUser().Id)
    {
      var builder = new IncorrectParameterMessageBuilder(context.GetTargetChatId());
      var report = new Report(Result.CanBeFixed, builder);
      return report;
    }
    sirenaContainer.Set(sirena);
    return new Report(Result.Success, null);
  }
}