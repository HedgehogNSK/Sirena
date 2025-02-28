using Hedgey.Localization;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Base.Interfaces;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using System.Globalization;

namespace Hedgey.Sirena.Bot;
public abstract class MessageBuilder : LocalizedBaseRequestBuilder, ISendMessageBuilder
{
  protected MessageBuilder(long chatId, CultureInfo info, ILocalizationProvider localizationProvider)
  : base(chatId, info, localizationProvider) { }

  public override abstract SendMessage Build();
  protected SendMessage CreateDefault(string message, IReplyMarkup? replyMarkup = null)
    => MarkupShortcuts.CreateDefaultMessage(ChatID, message, replyMarkup);
}