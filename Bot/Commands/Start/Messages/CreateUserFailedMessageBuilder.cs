using Hedgey.Localization;
using Hedgey.Structure.Factory;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Base.Interfaces;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;
using System.Globalization;

namespace Hedgey.Sirena.Bot;

public class CreateUserFailedMessageBuilder : MessageBuilder
{
  private readonly IRequestContext context;

  public CreateUserFailedMessageBuilder(IRequestContext context, CultureInfo info
  , ILocalizationProvider localizationProvider)
   : base(context.GetChat().Id, info, localizationProvider)
  {
    this.context = context;
  }

  public override SendMessage Build()
  {
    string request = context.GetQuery();
    IReplyMarkup markup = KeyboardBuilder.CreateInlineKeyboard()
      .BeginRow().AddRetryButton(Info, request).EndRow()
      .ToReplyMarkup();

    string message = Localize("command.start.create-user.fail");
    return CreateDefault(message, markup);
  }
  public class Factory(ILocalizationProvider localizationProvider)
   : IFactory<IRequestContext, ISendMessageBuilder>
  {
    public ISendMessageBuilder Create(IRequestContext context)
    {
      CultureInfo info = context.GetCultureInfo();
      return new CreateUserFailedMessageBuilder(context, info, localizationProvider);
    }
  }
}
