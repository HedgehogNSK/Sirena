using Hedgey.Blendflake;
using Hedgey.Utilities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Hedgey.Sirena.Database;

[BsonIgnoreExtraElements]
public class SirenRepresentation
{
  [BsonId]
  public ObjectId Id { get; set; }
  /// <summary>
  /// Search id
  /// </summary>
  private ulong sid;
  [BsonElement("sid"), BsonRepresentation(BsonType.Int64)]
  public required ulong SID
  {
    get => sid;
    init
    {
      sid = value;
      Hash = NotBase64URL.From(SID);
      ShortHash = HashUtilities.Shortify(Hash);
    }
  }
  [BsonIgnore]
  public string Hash { get; private set; } = string.Empty;
  [BsonIgnore]
  public string ShortHash { get; private set; } = string.Empty;
  [BsonElement("ownerid"), BsonRepresentation(BsonType.Int64)]
  public long OwnerId { get; set; }
  [BsonIgnore]
  public string OwnerNickname { get; set; } = string.Empty;
  [BsonElement("title")]
  public string Title { get; set; } = string.Empty;
  [BsonElement("count")]
  public int UseCount { get; set; }
  [BsonRepresentation(BsonType.Int64)]
  [BsonElement("listener")]
  public long[] Listener { get; set; } = [];
  [BsonElement("muted")]
  public MutedInfo[] Muted { get; set; } = [];
  [BsonRepresentation(BsonType.Int64)]
  [BsonElement("responsible")]
  public long[] Responsible { get; set; } = [];
  [BsonElement("requests")]
  public Request[] Requests { get; set; } = [];
  [BsonElement("last_call"), BsonIgnoreIfNull(true)]
  public CallInfo? LastCall { get; set; }

  public override string ToString()
  {
    return $"\\[`{ShortHash}`] {Title}";
  }

  public bool CanBeCalledBy(long uid)
  {
    //If user don't have right to call sirens or there is no listeners
    if ((OwnerId != uid && !Responsible.Contains(uid)) || Listener.Length == 0)
      return false;

    //If no one mutes anyone for the sirena then user can call it
    if (!Muted.Any()) return true;

    //Count of listeners who didn't mute the user
    int actualListeners = Listener.Length - Muted.Count(x => x.MutedUID == uid);

    //If the user is not owner he is also a listener so we have to subtract himself from the count
    if (uid != OwnerId)
      actualListeners -= 1;

    ArgumentOutOfRangeException.ThrowIfLessThan(actualListeners, 0, nameof(actualListeners));

    return actualListeners != 0;
  }

  public class Request(long uid, string message)
  {
    [BsonElement("user_id")]
    public long UID { get; } = uid;
    [BsonElement("message")]
    public string Message { get; } = message;
    public override int GetHashCode() => UID.GetHashCode();
    public override bool Equals(object? obj)
    {
      if (obj is Request other)
        return other.UID == UID;
      return false;
    }
  }
  public record class CallInfo(
    [property: BsonElement("caller"), BsonRepresentation(BsonType.Int64)] long Caller
    , [property: BsonElement("date"), BsonRepresentation(BsonType.DateTime)] DateTimeOffset Date)
  { }

  public record class MutedInfo(
    [property: BsonElement("user_id"), BsonRepresentation(BsonType.Int64)] long UID,
    [property: BsonElement("muted_id"), BsonRepresentation(BsonType.Int64)] long MutedUID)
  { }
}