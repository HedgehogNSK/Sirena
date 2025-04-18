using Hedgey.Sirena.Entities;

namespace Hedgey.Sirena.Bot.Operations
{
  public interface IDeleteSirenaOperation
  {
    IObservable<SirenaData> Delete(long userId, ulong sirenaId);
  }
}