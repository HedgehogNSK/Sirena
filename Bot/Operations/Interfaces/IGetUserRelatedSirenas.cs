using Hedgey.Sirena.Database;

namespace Hedgey.Sirena.Bot.Operations;

public interface IGetUserRelatedSirenas
{
  /// <summary>
  /// Loads Sirenas that user able to activate.
  /// User has to be an owner or be set as responsible by the owner of the Sirena
  /// </summary>
  /// <param name="uid">Telegram User ID</param>
  /// <returns></returns>
  IObservable<IEnumerable<SirenRepresentation>> GetAvailableForCallSirenas(long uid);
  /// <summary>
  /// Loads Sirenas created by the user
  /// </summary>
  /// <param name="uid">Telegram User ID</param>
  /// <returns></returns>
  IObservable<IEnumerable<SirenRepresentation>> GetUserSirenas(long uid);
  /// <summary>
  /// Loads user Sirena by serial number
  /// </summary>
  /// <param name="uid">Telegram User ID</param>
  /// <param name="number">Sirena Serial Number</param>
  /// <returns></returns>
  IObservable<SirenRepresentation> GetUserSirena(long uid, int number);
  /// <summary>
  /// Loads Sirenas thas user has been subscribed on
  /// </summary>
  /// <param name="uid">Telegram User ID</param>
  IObservable<IEnumerable<SirenRepresentation>> GetSubscriptions(long uid);
  /// <summary>
  /// Loads user Sirenas which has at least one request to delegate rights
  /// for activation the Sirena by another user
  /// </summary>
  /// <param name="uid"></param>
  /// <returns></returns>
  IObservable<IEnumerable<SirenRepresentation>> GetSirenasWithRequests(long uid);
  /// <summary>
  /// Loads Sirena by ID if user is the owner
  /// Returns null in other way
  /// </summary>
  /// <param name="uid">User Telegram ID</param>
  /// <param name="sid">Sirena ID</param>
  /// <returns></returns>
  IObservable<SirenRepresentation> GetUserSirenaOrNull(long uid, ulong sid); 
}