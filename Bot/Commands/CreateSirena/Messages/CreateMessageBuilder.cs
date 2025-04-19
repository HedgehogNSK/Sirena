using Hedgey.Localization;
using Hedgey.Sirena.Entities;
using Hedgey.Structure.Factory;
using Hedgey.Telegram.Bot;
using RxTelegram.Bot;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;
using System.Globalization;

namespace Hedgey.Sirena.Bot;
public class CreateMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider localizationProvider, int minSymbols, int maxSymbols
  , string botName) 
  : MessageBuilder(chatId, info, localizationProvider), ISendMessageBuilder
{
  private readonly int minSymbols = minSymbols;
  private readonly int maxSymbols = maxSymbols;
  private readonly string botName = botName;
  private bool userIsSet;
  private bool isAllowed;
  private bool isTitleValid;
  private SirenaData? sirena;

  internal void SetUser(bool isSet)
  {
    userIsSet = isSet;
  }
  internal void SetSirena(SirenaData? sirena)
  {
    this.sirena = sirena;
  }
  public void IsUserAllowedToCreateSirena(bool isAllowed)
  {
    this.isAllowed = isAllowed;
  }
  public void IsTitleValid(bool isValid)
  {
    isTitleValid = isValid;
  }
  public override SendMessage Build()
  {
    string message;

    var keyboardBuilder = KeyboardBuilder.CreateInlineKeyboard().BeginRow();
    if (!userIsSet)
    {
      message = Localize("command.create_sirena.error.no_user");
      message = string.Format(message, ChatID);
    }
    else if (!isAllowed)
    {
      message = Localize("command.create_sirena.error.limit_reached");
    }
    else if (!isTitleValid)
    {
      message = Localize("command.create_sirena.error.no_title");
      message = string.Format(message, minSymbols, maxSymbols);
    }
    else if (sirena == null)
    {
      message = Localize("command.create_sirena.error.unknown");
    }
    else
    {
      message = Localize("command.create_sirena.success");
      var link = MarkupShortcuts.CreateStartReference(botName, sirena.ShortHash);
      message = string.Format(message, sirena.ShortHash) + link.Replace("_", "\\_");
      var copyLink = Localize("miscellaneous.copy_link");
      var copyId = Localize("miscellaneous.copy_id");
      keyboardBuilder.AddCopyText(copyLink, link)
      .AddCopyText(copyId, sirena.ShortHash)
      .EndRow().BeginRow();
    }
    var markup = keyboardBuilder.AddMenuButton(Info).EndRow().ToReplyMarkup();
    return CreateDefault(message, markup);
  }

  public class Factory(ILocalizationProvider localizationProvider, TelegramBot bot)
    : IFactory<IRequestContext, CreateMessageBuilder>
  {
    private readonly ILocalizationProvider localizationProvider = localizationProvider;
    private readonly string botName = bot.GetMe().GetAwaiter().GetResult().Username;

    public CreateMessageBuilder Create(IRequestContext context)
    {
      var chatID = context.GetChat().Id;
      var info = context.GetCultureInfo();
      return new CreateMessageBuilder(chatID, info, localizationProvider
     , ValidateTitleCommandStep.TITLE_MIN_LENGHT
     , ValidateTitleCommandStep.TITLE_MAX_LENGHT, botName);
    }
  }
}