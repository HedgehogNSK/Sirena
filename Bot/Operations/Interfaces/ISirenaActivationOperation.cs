using Hedgey.Sirena.Database;
using MongoDB.Bson;

namespace Hedgey.Sirena.Bot.Operations;

public interface ISirenaActivationOperation
{
  IObservable<SirenaActivation> LogInfo(ulong sirenaId, long userId);
  IObservable<bool> SetReceivers(SirenaActivation call, IEnumerable<long> receiversIds);
  IObservable<SirenaActivation> SetReaction(ObjectId callId, long userId, int emojiCode);
}