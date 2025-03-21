using Hedgey.Localization;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Structure.Factory;
using System.Globalization;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;
public class WelcomeMessageBuilderFactory(ILocalizationProvider localizationProvider)
 : IFactory<IRequestContext, UserStatistics, ISendMessageBuilder>
{
  const string messageKey = "command.start.welcome";
  public ISendMessageBuilder Create(IRequestContext context, UserStatistics userStats)
  {
    CultureInfo info = context.GetCultureInfo();
    long chatId = context.GetTargetChatId();
    return new MenuMessageBuilder(chatId, info, localizationProvider)
        .AddUserStatistics(userStats)
        .SetText(messageKey);
  }
}