using Hedgey.Sirena.Entities;

namespace Hedgey.Sirena.Bot.Operations;
public interface IFindSirenaOperation
{
  IObservable<SirenaData> Find(ulong sirenaId);
  IObservable<List<SirenaData>> Find(string keyPhrase);
}