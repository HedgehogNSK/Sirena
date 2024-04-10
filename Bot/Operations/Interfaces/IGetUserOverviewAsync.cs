namespace Hedgey.Sirena.Bot.Operations;

public interface IGetUserOverviewAsync
{
  Task<UserStatistics> Get(long uid);
}
