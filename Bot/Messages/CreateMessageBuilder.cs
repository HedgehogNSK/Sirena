using Hedgey.Sirena.Database;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;

namespace Hedgey.Sirena.Bot;
public class CreateMessageBuilder : MessageBuilder
{
  private bool userIsSet;
  private bool isAllowed;
  private bool isTitleValid;
  private int minSymbols;
  private int maxSymbols;
  private SirenRepresentation? sirena;

  public CreateMessageBuilder(long chatId) : base(chatId)
  {
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
  public void IsTitleValid(bool isValid, int minSymbols, int maxSymbols)
  {
    isTitleValid = isValid;
    this.minSymbols = minSymbols;
    this.maxSymbols = maxSymbols;
  }
  public override SendMessage Build()
  {
    string message;
    const string noUser = "We can't find user with id: {0}";
    const string offLimit = "*You've reached the limit!* You can't create new sirenas anymore.\nPlease remove one of your sirenas and try again.";
    const string emptyTitleWarning = "Please insert title of the Sirena. Title must be between {0} and {1} symbols long.\n\n _Alternatively you can create Sirena via command syntax:_\n`/create title_of_sirena`";
    const string success = "Signal has been created successfuly. It's ID: *'{0}'*. Share it to subscribers.";
    if (!userIsSet)
      message = string.Format(noUser, chatId);
    else if (!isAllowed)
      message = offLimit;
    else if (!isTitleValid)
      message = string.Format(emptyTitleWarning, minSymbols, maxSymbols);
    else if (sirena == null)
      message = "Unknown error. Sirena wasn't created.";
    else
      message = string.Format(success, sirena.Id);

    // var keyboardBuilder = KeyboardBuilder.CreateInlineKeyboard().BeginRow()
    // .AddCallbackData(searchTitle, searchCallback)
    // .AddCallbackData(subscribeTitle, subscribeCallback);

    // if (userSubscribed)
    // {
    //   var title = subscriptionsTitle + ((result != null && result.Subscriptions != 0) ?
    //         $" [{result.Subscriptions}]" : string.Empty);
    //   keyboardBuilder.AddCallbackData(title, subscriptionsCallback);
    // }

    // keyboardBuilder.EndRow()
    // .BeginRow()
    // .AddCallbackData(createTitle, createCallback);

    // if (userHasSirenas)
    // {
    //   var title = listTitle + ((result != null && result.SirenasCount != 0) ?
    //         $" [{result.SirenasCount}]" : string.Empty);
    //   keyboardBuilder.AddCallbackData(title, listCallback);
    // }

    // IReplyMarkup markup = new InlineKeyboardMarkup()
    // {
    //   InlineKeyboard = keyboardBuilder.EndRow().Build()
    // };

    return new SendMessage
    {
      ChatId = chatId,
      Text = message,
      ReplyMarkup = null,
      ProtectContent = true,
      DisableNotification = true,
    };
  }
}