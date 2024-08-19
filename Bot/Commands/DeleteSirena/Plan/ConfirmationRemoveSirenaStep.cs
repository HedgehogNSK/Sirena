using Hedgey.Localization;
using Hedgey.Sirena.Database;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class ConfirmationRemoveSirenaStep : DeleteSirenaStep
{
  private readonly ILocalizationProvider localizationProvider;

  public ConfirmationRemoveSirenaStep(NullableContainer<SirenRepresentation> sirenaContainer,
ILocalizationProvider localizationProvider)
   : base(sirenaContainer)
  {
    this.localizationProvider = localizationProvider;
  }

  public override IObservable<Report> Make(IRequestContext context)
  {
    Report report;
    var info = context.GetCultureInfo();
    var param = context.GetArgsString();
    if (!bool.TryParse(param, out bool value))
    {
      long chatId = context.GetTargetChatId();
      var messageBuilder = new ConfirmRemoveSirenaMessageBuilder(chatId, info, localizationProvider, sirenaContainer.Get());
      report = new Report(Result.Wait, messageBuilder);
    }
    else
    {
      report = new Report(value ? Result.Success : Result.Canceled);
    }

    return Observable.Return(report);
  }
}