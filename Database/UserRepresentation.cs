using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Hedgey.Sirena.Database;
[Serializable]
public class UserRepresentation
{
  [BsonId,BsonRepresentation(BsonType.Int64)]
  public long UID { get; set; }
  [BsonElement("chatid"),BsonRepresentation(BsonType.Int64)]
  public long ChatID { get; set; }
  [BsonElement("owner")]
  public ObjectId[] Owner { get; set; }=[];
}