using Hedgey.Sirena.Database;

namespace Hedgey.Sirena.Bot.Operations;

public interface IUpdateSirenaOperation{
  IObservable<SirenRepresentation> UpdateLastCall(ulong sirenaId, SirenRepresentation.CallInfo  callInfo);
  IObservable<bool> UpdateDefault(ulong sirenaId);
}