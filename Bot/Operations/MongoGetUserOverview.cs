using Hedgey.Sirena.Database;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Hedgey.Sirena.Bot.Operations;
public class MongoGetUserOverview : IGetUserOverviewAsync
{
  private readonly IMongoCollection<SirenRepresentation> sirens;

  public MongoGetUserOverview(IMongoCollection<SirenRepresentation> sirens)
  {
    this.sirens = sirens;
  }

  public async Task<UserStatistics> Get(long userId)
  {
    var query = sirens.AsQueryable()
    .Where(_sirena => _sirena.OwnerId == userId
                      || _sirena.Listener.Any(x => x == userId)
                      || _sirena.Responsible.Any(x => x == userId))
    .GroupBy(keySelector: x => true,
      resultSelector: (_, _sirens) => new UserStatistics
        {
          SirenasCount = _sirens.Sum(x => x.OwnerId == userId ? 1 : 0),
          Subscriptions = _sirens.Sum(_sirena => (_sirena.Listener ?? new long[] { }).Contains(userId) ? 1 : 0),
          Responsible = _sirens.Sum(_sirena =>(_sirena.Responsible is Array) && (_sirena.Responsible ?? new long[] { }).Contains(userId) ? 1 : 0)
        });

    return await query.FirstOrDefaultAsync();
  }
}


public class UserStatistics
{
  public int SirenasCount { get; set; }
  public int Subscriptions { get; set; }
  public int Responsible { get; set; }
}