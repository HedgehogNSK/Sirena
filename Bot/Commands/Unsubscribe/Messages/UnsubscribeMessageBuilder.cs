using Hedgey.Localization;
using Hedgey.Structure.Factory;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;
using System.Globalization;

namespace Hedgey.Sirena.Bot;

internal class UnsubscribeMessageBuilder : LocalizedMessageBuilder
{
  private readonly ulong sirenaId;
  private bool isSuccess;

  public UnsubscribeMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider localizationProvider, ulong sirenaId, bool isSuccess) 
  : base(chatId, info, localizationProvider)
  {
    this.sirenaId = sirenaId;
    this.isSuccess = isSuccess;
  }
  public override SendMessage Build()
  {
    string message;
    if (isSuccess)
    {
      message = Localize("command.unsubscribe.success");
      message = string.Format(message, sirenaId);
      return CreateDefault(message, MarkupShortcuts.CreateMenuButtonOnlyMarkup(Info));
    }
    else
    {
      message = Localize("command.unsubscribe.fail");
      message = string.Format(message, sirenaId);
      string unsubscribeTitle = Localize("menu.buttons.anotherTry.title");
      var replyMarkup = KeyboardBuilder.CreateInlineKeyboard()
      .BeginRow().AddMenuButton(Info)
      .AddCallbackData(unsubscribeTitle, '/' + UnsubscribeCommand.NAME).EndRow()
      .ToReplyMarkup();
      return CreateDefault(message, replyMarkup);
    }
  }

  public class Factory(ILocalizationProvider localizationProvider)
    : IFactory<IRequestContext, ulong, bool, IMessageBuilder>
  {
    public IMessageBuilder Create(IRequestContext context, ulong sirenaId, bool isSuccess)
    {
      var chatId = context.GetTargetChatId();
      var info = context.GetCultureInfo();
      return new UnsubscribeMessageBuilder(chatId, info, localizationProvider,sirenaId, isSuccess);
    }
  }
}