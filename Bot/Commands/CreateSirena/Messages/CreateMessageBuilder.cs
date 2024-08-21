using Hedgey.Localization;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;
using SimpleInjector;
using System.Globalization;

namespace Hedgey.Sirena.Bot;
public class CreateMessageBuilder : LocalizedMessageBuilder, IMessageBuilder
{
  private bool userIsSet;
  private bool isAllowed;
  private bool isTitleValid;
  private int minSymbols;
  private int maxSymbols;
  private SirenRepresentation? sirena;

  public CreateMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider localizationProvider, int minSymbols, int maxSymbols)
   : base(chatId, info, localizationProvider)
  {
    this.minSymbols = minSymbols;
    this.maxSymbols = maxSymbols;
  }
  internal void SetUser(UserRepresentation? representation)
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
      message = Localize("command.create_sirena.error.no_user");
      message = string.Format(message, chatId);
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
      message = string.Format(message, sirena.Id);
    }
    var markup = keyboardBuilder.AddMenuButton(Info).EndRow().ToReplyMarkup();
    return CreateDefault(message, markup);
  }

  public class Factory(ILocalizationProvider localizationProvider) 
  : IFactory<IRequestContext, CreateMessageBuilder>
  {
    public CreateMessageBuilder Create(IRequestContext context)
    {
      var chatID = context.GetChat().Id;
      var info = context.GetCultureInfo();
      return new CreateMessageBuilder(chatID, info, localizationProvider
     , ValidateTitleCommandStep.TITLE_MIN_LENGHT
     , ValidateTitleCommandStep.TITLE_MAX_LENGHT);
    }
  }
}