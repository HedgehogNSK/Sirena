using Hedgey.Sirena.Database;

namespace Hedgey.Sirena.Bot.Operations;

public interface IGetUserRelatedSirenas
{
  /// <summary>
  /// Loads Sirenas that user able to activate.
  /// User has to be an owner or be set as responsible by the owner of the Sirena
  /// </summary>
  /// <param name="userId">Telegram User ID</param>
  /// <returns></returns>
  IObservable<IEnumerable<SirenRepresentation>> GetAvailableForCallSirenas(long userId);
  /// <summary>
  /// Loads Sirenas created by the user
  /// </summary>
  /// <param name="userId">Telegram User ID</param>
  /// <returns></returns>
  IObservable<IEnumerable<SirenRepresentation>> GetUserSirenas(long userId);
  /// <summary>
  /// Loads user Sirena by serial number
  /// </summary>
  /// <param name="userId">Telegram User ID</param>
  /// <param name="number">Sirena Serial Number</param>
  /// <returns></returns>
  IObservable<SirenRepresentation> GetUserSirena(long userId, int number);
  /// <summary>
  /// Loads Sirenas thas user has been subscribed on
  /// </summary>
  /// <param name="userId">Telegram User ID</param>
  IObservable<IEnumerable<SirenRepresentation>> GetSubscriptions(long userId);
  /// <summary>
  /// Loads user Sirenas which has at least one request to delegate rights
  /// for activation the Sirena by another user
  /// </summary>
  /// <param name="userId"></param>
  /// <returns></returns>
  IObservable<IEnumerable<SirenRepresentation>> GetSirenasWithRequests(long userId);
  /// <summary>
  /// Loads Sirena by ID if user is the owner
  /// Returns null in other way
  /// </summary>
  /// <param name="userId">User Telegram ID</param>
  /// <param name="sirenaId">Sirena ID</param>
  /// <returns></returns>
  IObservable<SirenRepresentation> GetUserSirenaOrNull(long userId, ulong sirenaId); 
}