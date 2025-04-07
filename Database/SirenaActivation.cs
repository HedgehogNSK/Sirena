using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Hedgey.Sirena.Database;
public record SirenaActivation(
    [property: BsonElement("sid"), BsonRepresentation(BsonType.Int64)] ulong SirenaId,
    [property: BsonElement("caller"), BsonRepresentation(BsonType.Int64)] long Caller,
    [property: BsonElement("receivers"), BsonIgnoreIfNull] SirenaActivation.Receiver[]? Receivers = null)
{
  [BsonId, BsonIgnoreIfDefault] public ObjectId Id { get; init; }

  //Despite Id.CreationTime is DateTime, it should be created in UTC timezone
  [BsonElement("date"), BsonRepresentation(BsonType.DateTime)]
  public DateTimeOffset Date => Id.CreationTime;

  public record Receiver(
      [property: BsonElement("id"), BsonRepresentation(BsonType.Int64)] long UserId,
      [property: BsonElement("reaction"), BsonIgnoreIfDefault] int? Reaction = null);
}