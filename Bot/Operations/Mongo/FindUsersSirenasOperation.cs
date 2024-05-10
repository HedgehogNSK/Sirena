using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Hedgey.Sirena.Database;
using MongoDB.Driver;

namespace Hedgey.Sirena.Bot.Operations.Mongo;

public class FindUsersSirenasOperation : IFindUserSirenasOperation
{
  private readonly IMongoCollection<SirenRepresentation> sirens;

  public FindUsersSirenasOperation(IMongoCollection<SirenRepresentation> sirenCollection)
  {
    this.sirens = sirenCollection;
  }

  public IObservable<IEnumerable<SirenRepresentation>> Find(long uid)
  {
    var filter = Builders<SirenRepresentation>.Filter.Eq(x => x.OwnerId, uid);
    return sirens.Find(filter).ToListAsync().ToObservable();
  }
  public IObservable<SirenRepresentation> FindBySerialNumber(long uid, int number)
  {
    --number;// switch number to id
    if (number < 0)
      return Observable.Throw<SirenRepresentation>(new ArgumentException("Serial number has to be positive",nameof(number)));

    var filterSiren = Builders<SirenRepresentation>.Filter.Eq(x => x.OwnerId, uid);
    return sirens.Find(filterSiren).Skip(number).FirstOrDefaultAsync().ToObservable();
  }
}
