using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Hedgey.Sirena.Entities;
[Serializable]
public class UserData
{
  [BsonId, BsonRepresentation(BsonType.Int64)]
  public long UID { get; set; }
  [BsonElement("chatid"), BsonRepresentation(BsonType.Int64)]
  public long ChatID { get; set; }
  [BsonElement("owner")]
  public ulong[] Owner { get; set; } = [];}