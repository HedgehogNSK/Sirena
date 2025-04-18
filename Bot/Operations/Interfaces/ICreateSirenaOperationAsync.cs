using Hedgey.Sirena.Entities;

namespace Hedgey.Sirena.Bot.Operations;
public interface ICreateSirenaOperationAsync
{
  Task<SirenaData> CreateAsync(long uid, string sirenName);
}