using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Hedgey.Sirena.Database;

public class SirenRepresentation
{
  [BsonId]
  public ObjectId Id { get; set; }
  [BsonElement("ownerid"), BsonRepresentation(BsonType.Int64)]
  public long OwnerId { get; set; }
  public string OwnerNickname { get; set; } = string.Empty;
  [BsonElement("title")]
  public string Title { get; set; } = string.Empty;
  [BsonElement("count")]
  public int UseCount { get; set; }
  [BsonRepresentation(BsonType.Int64)]
  [BsonElement("listener")]
  public long[] Listener { get; set; } = [];
  [BsonRepresentation(BsonType.Int64)]
  [BsonElement("responsible")]
  public long[] Responsible { get; set; } = [];
  [BsonElement("requests")]
  public Request[] Requests { get; set; } = [];
  [BsonElement("last_call"), BsonIgnoreIfNull(true)]
  public CallInfo? LastCall { get; set; }

  public override string ToString()
  {
    return $"{Title} (`{Id}`)";
  }

  public bool CanBeCalledBy(long uid)
  => OwnerId == uid || Responsible.Contains(uid);

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
    public override int GetHashCode() => UID.GetHashCode();
    public override bool Equals(object? obj)
    {
      if (obj is Request other)
        return other.UID == UID;
      return false;
    }
  }
  public class CallInfo
  {
    public CallInfo(long uid, DateTimeOffset now)
    {
      Caller = uid;
      Date = now;
    }
    [BsonElement("caller"), BsonRepresentation(BsonType.Int64)]
    public long Caller { get; internal set; }
    [BsonElement("date"), BsonRepresentation(BsonType.DateTime)]
    public DateTimeOffset Date { get; internal set; }
  }
}