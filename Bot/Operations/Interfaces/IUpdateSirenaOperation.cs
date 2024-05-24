using Hedgey.Sirena.Database;
using MongoDB.Bson;

namespace Hedgey.Sirena.Bot.Operations;

public interface IUpdateSirenaOperation{
  IObservable<SirenRepresentation> UpdateLastCall(ObjectId sirenId, SirenRepresentation.CallInfo  callInfo);
}