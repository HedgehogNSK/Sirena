using Hedgey.Localization;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Structure.Factory;
using System.Globalization;

namespace Hedgey.Sirena.Bot;
public class WelcomeMessageBuilderFactory(ILocalizationProvider localizationProvider)
 : IFactory<IRequestContext, UserStatistics, IMessageBuilder>
{
  const string messageKey = "command.start.welcome";
  public IMessageBuilder Create(IRequestContext context, UserStatistics userStats)
  {
    CultureInfo info = context.GetCultureInfo();
    long chatId = context.GetTargetChatId();
    return new MenuMessageBuilder(chatId, info, localizationProvider)
        .AddUserStatistics(userStats)
        .SetText(messageKey);
  }
}