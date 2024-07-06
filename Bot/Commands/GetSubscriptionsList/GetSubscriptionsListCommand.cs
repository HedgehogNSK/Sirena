using Hedgey.Localization;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;
public class GetSubscriptionsListCommand : AbstractBotCommmand, IBotCommand//, IDisposable
{
  public const string NAME = "subscriptions";
  public const string DESCRIPTION = "Displays your current subscriptions.";
  CompositeDisposable disposables = new CompositeDisposable();
  private readonly IMessageSender messageSender;
  private readonly IGetUserRelatedSirenas findSirena;
  private readonly IGetUserInformation getUserInformation;
  private readonly ILocalizationProvider localizationProvider;

  public GetSubscriptionsListCommand(IMessageSender messageSender
  , IGetUserRelatedSirenas findSirena, IGetUserInformation getUserInformation
  , ILocalizationProvider localizationProvider)
    : base(NAME, DESCRIPTION)
  {
    this.messageSender = messageSender;
    this.findSirena = findSirena;
    this.getUserInformation = getUserInformation;
    this.localizationProvider = localizationProvider;
  }

  public override void Execute(IRequestContext context)
  {
    long uid = context.GetUser().Id;

    IDisposable userSubscriptionsStream = findSirena.GetSubscriptions(uid)
      .SelectMany(_sirenas => _sirenas)
      .SelectMany(_sirena => getUserInformation.GetNickname(_sirena.OwnerId)
          .Select(_nick => (_sirena, _nick)))
      .ToArray()
      .Subscribe(_subscriptions => ProcessResult(_subscriptions, context));

    disposables.Add(userSubscriptionsStream);
  }

  private void ProcessResult((SirenRepresentation _sirena, string _nick)[] subscriptions, IRequestContext context)
  {
    long chatId = context.GetChat().Id;
    var info = context.GetCultureInfo();
    MessageBuilder builder = new SubscriptionsMesssageBuilder(chatId, info, localizationProvider, subscriptions);
    messageSender.Send(builder.Build());
  }

  public void Dispose()
  {
    disposables?.Dispose();
  }
  
  public class Installer(SimpleInjector.Container container)
   : CommandInstaller<GetSubscriptionsListCommand>(container)
  {
    public override void Install()
    {
      base.Install();
    }
  }
}