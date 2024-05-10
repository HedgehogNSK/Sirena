using Hedgey.Sirena.Database;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Reactive.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Hedgey.Sirena.Bot.Operations.Mongo;

public class FindSirenaOperation : IFindSirenaOperation
{
  private readonly IMongoCollection<SirenRepresentation> sirens;

  public FindSirenaOperation(IMongoCollection<SirenRepresentation> sirenCollection)
  {
    this.sirens = sirenCollection;
  }

  public IObservable<SirenRepresentation> Find(ObjectId id)
  {
    var filterSiren = Builders<SirenRepresentation>.Filter.Eq(x => x.Id, id);
    return sirens.Find(filterSiren).FirstOrDefaultAsync().ToObservable();
  }

  public IObservable<List<SirenRepresentation>> Find(string keyPhrase)
  {
    var formatedKey = Regex.Escape(keyPhrase);
    var pattern = new Regex(formatedKey, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);
    var bsonRegex = new BsonRegularExpression(pattern);
    var filter = Builders<SirenRepresentation>.Filter.Regex(x => x.Title, bsonRegex);
    return sirens.Find(filter).ToListAsync().ToObservable();
  }
}