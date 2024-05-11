using Hedgey.Sirena.Database;
using MongoDB.Bson;

namespace Hedgey.Sirena.Bot.Operations;

public interface ISubscribeToSirenaOperation
{
  IObservable<SirenRepresentation> Subscribe(long userId, ObjectId sirenaId);
}