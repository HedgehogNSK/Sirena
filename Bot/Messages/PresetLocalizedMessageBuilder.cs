using Hedgey.Localization;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using System.Globalization;

namespace Hedgey.Sirena.Bot;

public class PresetLocalizedMessageBuilder : LocalizedMessageBuilder
{
  protected readonly string key;
  private readonly object[] args;

  public PresetLocalizedMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider localizationProvider, string localizationKey
  , object[] args) 
    : base(chatId, info, localizationProvider)
  {
    key = localizationKey;
    this.args = args;
  }

  public override SendMessage Build()
  {
    string message = Localize(key);
    if(args.Length!=0)
      message =string.Format(message, args);
    return CreateDefault(message);
  }
}