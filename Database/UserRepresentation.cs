using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Hedgey.Sirena.Database;
[Serializable]
public class UserRepresentation
{
  [BsonId, BsonRepresentation(BsonType.Int64)]
  public long UID { get; set; }
  [BsonElement("chatid"), BsonRepresentation(BsonType.Int64)]
  public long ChatID { get; set; }
  [BsonElement("owner")]
  public ObjectId[] Owner { get; set; } = [];
  [BsonElement("muted")]
  public MuteInfo[] Muted { get; internal set; } = [];

  public class MuteInfo
  {

    public MuteInfo(long targetID, ObjectId sirenaId)
    {
      UID = targetID;
      SirenaID = sirenaId;
    }
    [BsonElement("user_id"), BsonRepresentation(BsonType.Int64)]
    public long UID { get; set; }
    [BsonElement("sirena_id")]
    public ObjectId SirenaID { get; set; }

    public override bool Equals(object? obj)
    {
      if (!(obj is MuteInfo otherInfo)) return false;
      if (otherInfo.UID != this.UID || !otherInfo.SirenaID.Equals(SirenaID)) return false;
      return true;
    }
    public override int GetHashCode()
    {
      unchecked
      {
        const int primeMul = 23;
        return (int)UID * primeMul + SirenaID.GetHashCode();
      }
    }
  }
}