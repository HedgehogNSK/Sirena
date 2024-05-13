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
  private readonly IGetUserRelatedSirenas getUserSirenasOperation;

  public FindRemoveSirenaStep(Container<IRequestContext> contextContainer
  , NullableContainer<SirenRepresentation> sirenaContainer
  , IFindSirenaOperation findSirenaOperation
  , IGetUserRelatedSirenas getUserSirenasOperation)
  : base(contextContainer, sirenaContainer)
  {
    this.findSirenaOperation = findSirenaOperation;
    this.getUserSirenasOperation = getUserSirenasOperation;
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
      return getUserSirenasOperation.GetUserSirenas(uid)
        .Select(_sireans => new RemoveSirenaMenuMessageBuilder(chatId,_sireans))
        .Select(_removeMenuBuilder => new Report(Result.Wait, _removeMenuBuilder));
    }
    else if (ObjectId.TryParse(param, out var id))
    {
      observableSirena = findSirenaOperation.Find(id);
    }
    else if (int.TryParse(param, out number))
    {
      observableSirena = getUserSirenasOperation.GetUserSirena(uid, number);
    }
    else
    {
      var builder = new IncorrectParameterMessageBuilder(chatId);
      var report = new Report(Result.Wait, builder);
      return Observable.Return(report);
    }

    return observableSirena.Select((_sirena) => ProcessRequestById(_sirena, context));
  }

  private Report ProcessRequestById(SirenRepresentation? sirena, IRequestContext context)
  {
    if (sirena == null || sirena.OwnerId != context.GetUser().Id)
    {
      var builder = new IncorrectParameterMessageBuilder(context.GetTargetChatId());
      var report = new Report(Result.Wait, builder);
      return report;
    }
    sirenaContainer.Set(sirena);
    return new Report(Result.Success, null);
  }
}