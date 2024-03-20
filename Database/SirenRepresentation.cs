using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Hedgey.Sirena.Database;

public class SirenRepresentation
{
  [BsonId]
  public ObjectId Id { get; set; }
  [BsonElement("ownerid"), BsonRepresentation(BsonType.Int64)]
  public long OwnerId { get; set; }
  [BsonElement("title")]
  public string Title { get; set; } = string.Empty;
  [BsonElement("count")]
  public int UseCount { get; set; }
  [BsonRepresentation(BsonType.Int64)]
  [BsonElement("listener")]
  public long[] Listener { get; internal set; } = [];
  [BsonRepresentation(BsonType.Int64)]
  [BsonElement("responsible")]
  public long[] Responsible { get; internal set; } = [];
  [BsonElement("requests")]
  public Request[] Requests { get; internal set; } = [];
  [BsonElement("last_call"),BsonRepresentation(BsonType.DateTime)]
  public DateTimeOffset LastCall { get; internal set; }

  public class Request
  {
    [BsonElement("user_id")]
    public long UID { get; set; }
    [BsonElement("message")]
    public string Message { get; set; } = string.Empty;
 
  }
  public override string ToString()
  {
    const string message= "{{ {0} : {1} }}";
    return string.Format(message, Id, Title);
  }
}