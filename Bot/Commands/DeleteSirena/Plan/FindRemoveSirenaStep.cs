using Hedgey.Extensions;
using Hedgey.Localization;
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
  private readonly ILocalizationProvider localizationProvider;

  public FindRemoveSirenaStep(NullableContainer<SirenRepresentation> sirenaContainer
  , IFindSirenaOperation findSirenaOperation
  , IGetUserRelatedSirenas getUserSirenasOperation,
ILocalizationProvider localizationProvider)
  : base(sirenaContainer)
  {
    this.findSirenaOperation = findSirenaOperation;
    this.getUserSirenasOperation = getUserSirenasOperation;
    this.localizationProvider = localizationProvider;
  }

  public override IObservable<Report> Make(IRequestContext context)
  {
    User botUser = context.GetUser();
    long uid = botUser.Id;
    long chatId = context.GetTargetChatId();
    var info = context.GetCultureInfo();
    string param = context.GetArgsString().GetParameterByNumber(0);
    IObservable<SirenRepresentation?> observableSirena;
    int number = 0;
    if (string.IsNullOrEmpty(param))
    {
      return getUserSirenasOperation.GetUserSirenas(uid)
        .Select(_sireans => new RemoveSirenaMenuMessageBuilder(chatId, info, localizationProvider, _sireans))
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
      var builder = new IncorrectParameterMessageBuilder(chatId, info, localizationProvider);
      var report = new Report(Result.Wait, builder);
      return Observable.Return(report);
    }

    return observableSirena.Select((_sirena) => ProcessRequestById(_sirena, context));
  }

  private Report ProcessRequestById(SirenRepresentation? sirena, IRequestContext context)
  {
    if (sirena == null || sirena.OwnerId != context.GetUser().Id)
    {
      var info = context.GetCultureInfo();
      var builder = new IncorrectParameterMessageBuilder(context.GetTargetChatId(), info, localizationProvider);
      var report = new Report(Result.Wait, builder);
      return report;
    }
    sirenaContainer.Set(sirena);
    return new Report(Result.Success, null);
  }
}