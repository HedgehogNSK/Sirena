using Hedgey.Sirena.Database;

namespace Hedgey.Sirena.Bot;

public record struct ServiceMessageData(long receiverId, SirenRepresentation sirena, SirenaActivation callInfo);