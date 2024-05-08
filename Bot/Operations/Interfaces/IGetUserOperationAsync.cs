using Hedgey.Sirena.Database;

public interface IGetUserOperationAsync{
  Task<UserRepresentation> GetAsync(long id);
}