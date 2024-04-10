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
  public long[] Listener { get;  set; } = [];
  [BsonRepresentation(BsonType.Int64)]
  [BsonElement("responsible")]
  public long[] Responsible { get;  set; } = [];
  [BsonElement("requests")]
  public Request[] Requests { get;  set; } = [];
  [BsonElement("last_call"), BsonIgnoreIfNull(true)]
  public CallInfo? LastCall { get;  set; }

  public override string ToString()
  {
    const string message= "{{{0}|{1}}}";
    return string.Format(message, Id, Title);
  }

  public class Request
  {
    public Request(long uid, string message)
    {
      UID = uid;
      Message = message;
    }
    [BsonElement("user_id")]
    public long UID { get; set; }
    [BsonElement("message")]
    public string Message { get; set; } = string.Empty;
  }
  public class CallInfo{
    public CallInfo(long uid, DateTimeOffset now)
    {
      Caller =uid;
      Date =now;
    }
    [BsonElement("caller"),BsonRepresentation(BsonType.Int64)]
  public long Caller  { get; internal set; }
    [BsonElement("date"),BsonRepresentation(BsonType.DateTime)]
  public DateTimeOffset Date { get; internal set; }
  }
}