using Hedgey.Sirena.Entities;

namespace Hedgey.Sirena.Bot.Operations;
public interface IGetUserOperationAsync{
  Task<UserData?> GetAsync(long id);
}