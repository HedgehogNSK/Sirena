using Hedgey.Sirena.Entities;

namespace Hedgey.Sirena.Bot;

public record struct ServiceMessageData(long receiverId, SirenaData sirena, SirenaActivation callInfo);