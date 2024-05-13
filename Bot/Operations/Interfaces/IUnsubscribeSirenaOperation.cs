using MongoDB.Bson;

namespace Hedgey.Sirena.Bot.Operations;

public interface IUnsubscribeSirenaOperation{
  IObservable<bool> Unsubscribe(long userId, ObjectId sirenaId);
}