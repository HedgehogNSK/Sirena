using Hedgey.Sirena.Database;

namespace Hedgey.Sirena.Bot.Operations;
public interface ICreateSirenaOperationAsync
{
  Task<SirenRepresentation> CreateAsync(long uid, string sirenName);
}