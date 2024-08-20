using Hedgey.Extensions;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using MongoDB.Bson;
using RxTelegram.Bot.Interface.BaseTypes;
using System.Data;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;
public class FindRemoveSirenaStep : DeleteSirenaStep
{
  private readonly IFindSirenaOperation findSirenaOperation;
  private readonly IGetUserRelatedSirenas getUserSirenasOperation;
  private readonly IFactory<IRequestContext, IEnumerable<SirenRepresentation>, RemoveSirenaMenuMessageBuilder> removeMenuMessageBuilderFactory;
  private readonly IFactory<IRequestContext, IncorrectParameterMessageBuilder> inorrectParamMessageBuilderFactory;

  public FindRemoveSirenaStep(NullableContainer<SirenRepresentation> sirenaContainer
  , IFindSirenaOperation findSirenaOperation
  , IGetUserRelatedSirenas getUserSirenasOperation
  , IFactory<IRequestContext, IEnumerable<SirenRepresentation>, RemoveSirenaMenuMessageBuilder> removeMenuMessageBuilderFactory
  , IFactory<IRequestContext, IncorrectParameterMessageBuilder> inorrectParamMessageBuilderFactory)
  : base(sirenaContainer)
  {
    this.findSirenaOperation = findSirenaOperation;
    this.getUserSirenasOperation = getUserSirenasOperation;
    this.removeMenuMessageBuilderFactory = removeMenuMessageBuilderFactory;
    this.inorrectParamMessageBuilderFactory = inorrectParamMessageBuilderFactory;
  }

  public override IObservable<Report> Make(IRequestContext context)
  {
    User botUser = context.GetUser();
    long uid = botUser.Id;
    var info = context.GetCultureInfo();
    string param = context.GetArgsString().GetParameterByNumber(0);
    IObservable<SirenRepresentation?> observableSirena;
    int number = 0;
    if (string.IsNullOrEmpty(param))
    {
      return getUserSirenasOperation.GetUserSirenas(uid)
        .Select(_sireans => removeMenuMessageBuilderFactory.Create(context, _sireans))
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
      var builder = inorrectParamMessageBuilderFactory.Create(context);
      var report = new Report(Result.Wait, builder);
      return Observable.Return(report);
    }

    return observableSirena.Select((_sirena) => ProcessRequestById(_sirena, context));
  }

  private Report ProcessRequestById(SirenRepresentation? sirena, IRequestContext context)
  {
    if (sirena == null || sirena.OwnerId != context.GetUser().Id)
    {
      var builder = inorrectParamMessageBuilderFactory.Create(context);
      var report = new Report(Result.Wait, builder);
      return report;
    }
    sirenaContainer.Set(sirena);
    return new Report(Result.Success, null);
  }
  public class Factory(IGetUserRelatedSirenas getUserSirenasOperation
  , IFindSirenaOperation findSirenaOperation
  , IFactory<IRequestContext, IncorrectParameterMessageBuilder> inorrectParamMessageBuilderFactory
  , IFactory<IRequestContext, IEnumerable<SirenRepresentation>, RemoveSirenaMenuMessageBuilder> removeMenuMessageBuilderFactory)
    : IFactory<NullableContainer<SirenRepresentation>, FindRemoveSirenaStep>
  {
    public FindRemoveSirenaStep Create(NullableContainer<SirenRepresentation> sirenaContainer)
    {
      return new FindRemoveSirenaStep(sirenaContainer, findSirenaOperation
      , getUserSirenasOperation, removeMenuMessageBuilderFactory
      , inorrectParamMessageBuilderFactory);
    }
  }
}