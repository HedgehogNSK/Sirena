using Hedgey.Localization;
using Hedgey.Structure.Factory;
using MongoDB.Bson;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;
using System.Globalization;

namespace Hedgey.Sirena.Bot;

internal class UnsubscribeMessageBuilder : LocalizedMessageBuilder
{
  private readonly ObjectId objectId;
  private bool isSuccess;

  public UnsubscribeMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider localizationProvider, ObjectId objectId, bool isSuccess) 
  : base(chatId, info, localizationProvider)
  {
    this.objectId = objectId;
    this.isSuccess = isSuccess;
  }
  public override SendMessage Build()
  {
    string message;
    if (isSuccess)
    {
      message = Localize("command.unsubscribe.success");
      message = string.Format(message, objectId);
      return CreateDefault(message, MarkupShortcuts.CreateMenuButtonOnlyMarkup(Info));
    }
    else
    {
      message = Localize("command.unsubscribe.fail");
      message = string.Format(message, objectId);
      string unsubscribeTitle = Localize("menu.buttons.anotherTry.title");
      var replyMarkup = KeyboardBuilder.CreateInlineKeyboard()
      .BeginRow().AddMenuButton(Info)
      .AddCallbackData(unsubscribeTitle, '/' + UnsubscribeCommand.NAME).EndRow()
      .ToReplyMarkup();
      return CreateDefault(message, replyMarkup);
    }
  }

  public class Factory(ILocalizationProvider localizationProvider)
    : IFactory<IRequestContext, ObjectId, bool, IMessageBuilder>
  {
    public IMessageBuilder Create(IRequestContext context, ObjectId objectId, bool isSuccess)
    {
      var chatId = context.GetTargetChatId();
      var info = context.GetCultureInfo();
      return new UnsubscribeMessageBuilder(chatId, info, localizationProvider,objectId, isSuccess);
    }
  }
}