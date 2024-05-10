using Hedgey.Sirena.Database;

namespace Hedgey.Sirena.Bot.Operations;
public interface IGetUserOperationAsync{
  Task<UserRepresentation> GetAsync(long id);
}