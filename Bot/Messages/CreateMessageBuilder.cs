using Hedgey.Localization;
using Hedgey.Sirena.Database;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;
using System.Globalization;

namespace Hedgey.Sirena.Bot;
public class CreateMessageBuilder : MessageBuilder
{
  private bool userIsSet;
  private bool isAllowed;
  private bool isTitleValid;
  private int minSymbols;
  private int maxSymbols;
  private SirenRepresentation? sirena;
  private readonly ILocalizationProvider localizationProvider;
  private readonly CultureInfo info;

  public CreateMessageBuilder(int minSymbols, int maxSymbols, ILocalizationProvider localizationProvider, CultureInfo info, long chatId) : base(chatId)
  {
    this.minSymbols = minSymbols;
    this.maxSymbols = maxSymbols;
    this.localizationProvider = localizationProvider;
    this.info = info;
  }
  internal void SetUser(UserRepresentation representation)
  {
    userIsSet = representation != null;
  }
  internal void SetSirena(SirenRepresentation? sirena)
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
      message = localizationProvider.Get("command.create_sirena.error.no_user", info);
      message = string.Format(message, chatId);
    }
    else if (!isAllowed)
    {
      message = localizationProvider.Get("command.create_sirena.error.limit_reached", info);
    }
    else if (!isTitleValid)
    {
      message = localizationProvider.Get("command.create_sirena.error.no_title", info);
      message = string.Format(message, minSymbols, maxSymbols);
    }
    else if (sirena == null)
    {
      message = localizationProvider.Get("command.create_sirena.error.unknown", info);
    }
    else
    {
      message = localizationProvider.Get("command.create_sirena.success", info);
      message = string.Format(message, sirena.Id);
    }
    var markup = keyboardBuilder.AddMenuButton().EndRow().ToReplyMarkup();
    return CreateDefault(message, markup);
  }
}