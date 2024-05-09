using System.Data;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Hedgey.Extensions;
using Hedgey.Sirena.Database;
using MongoDB.Bson;
using RxTelegram.Bot.Interface.BaseTypes;

namespace Hedgey.Sirena.Bot;

public class FindRemoveSirenaStep : DeleteSirenaStep
{
  private readonly FacadeMongoDBRequests requests;

  public FindRemoveSirenaStep(Container<IRequestContext> contextContainer
  , Container<SirenRepresentation> sirenaContainer, FacadeMongoDBRequests requests)
  : base(contextContainer, sirenaContainer)
  {
    this.requests = requests;
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
      return requests.GetOwnedSirenas(uid, chatId).ToObservable()
        .Select(_sireans => new RemoveSirenaMenuMessageBuilder(chatId,_sireans))
        .Select(_removeMenuBuilder => new Report(Result.CanBeFixed, _removeMenuBuilder));
    }
    else if (ObjectId.TryParse(param, out var id))
    {
      observableSirena = requests.GetSirenaById(id).ToObservable();
    }
    else if (int.TryParse(param, out number))
    {
      observableSirena = requests.GetSirenaBySerialNumber(uid, number).ToObservable();
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
// string messageText ;
//   User botUser = context.GetUser();
//   long uid = botUser.Id;
//   long chatId = context.GetChat().Id;
//   string param = context.GetArgsString().GetParameterByNumber(0);
//   ObjectId id = await request.GetSirenaId(uid, param);
//   if (id == ObjectId.Empty)
//   {
//     Program.messageSender.Send(chatId, wrongParameter);
//     return;
//   }
//   //Remove srien Id from the owner document
//   var filter = Builders<UserRepresentation>.Filter.Eq(x => x.UID, uid);
//   var userUpdate = Builders<UserRepresentation>.Update.Pull<ObjectId>(x => x.Owner, id);
//   var userUpdateResult = await usersCollection.UpdateOneAsync(filter, userUpdate);

//   //Remove siren from collection by ID
//   var sirenFilter = Builders<SirenRepresentation>.Filter.Eq(x => x.Id, id);
//   var result2 = await sirenCollection.FindOneAndDeleteAsync(sirenFilter);
//   messageText = result2 != null ? '*' + result2.Title + "* has been removed" :
//   "You don't have *sirena* with id: *" + id + '*';
//   Program.messageSender.Send(chatId, messageText);