using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;

namespace Hedgey.Sirena.Bot;

public class IncorrectParameterMessageBuilder : MessageBuilder
  {
    const string errorMessage = "To delete a Sirena, please insert *Sirena's ID or serial number* that is owned by you.";
    public IncorrectParameterMessageBuilder(long chatId)
    : base(chatId) { }

    public override SendMessage Build()
    {
      return CreateDefault(errorMessage);
    }
  }