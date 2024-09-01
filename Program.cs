using Hedgey.Extensions;
using Hedgey.Extensions.SimpleInjector;
using Hedgey.Extensions.Types;
using Hedgey.Sirena.Bot;
using Hedgey.Sirena.Bot.DI;
using MongoDB.Driver;
using RxTelegram.Bot;
using RxTelegram.Bot.Exceptions;
using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Callbacks;
using SimpleInjector;
using System.Collections;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;

namespace Hedgey.Sirena;
static internal class Program
{
  private static void TestDisplay(ulong id)
  {
    StringBuilder builder = new StringBuilder();
    Bit.Write(id);
    string result = Converter.ToBase64URL(id);
    Console.WriteLine(id + " " + builder.ToString() + " " + result);
    long number = Converter.FromBase64(result);
    Console.WriteLine("Opposite: " + number);
  }
  private static async Task Main(string[] args)
  {
    Container container = new Container();
    Installer installer = new CoreInstaller(container);
    installer.Install();
    installer = new SharedCommandServicesInstaller(container);
    installer.Install();
    InstallCommands(container);

    container.Verify();
    // Bit.Write(294669217524142L);
    // TestDisplay(294669217524142L);
    var idGen = container.GetInstance<IIDGenerator>();
    int i = 0;
    while (i != 2)
    {
      for (int jd = 0; jd != 110; ++jd)
      {
        var id = idGen.Get();
        var bytes = BitConverter.GetBytes(id);
        BitArray bits = new BitArray(bytes);
        StringBuilder builder = new StringBuilder();
        foreach (bool each in bits)
          builder.Append(each == false ? '0' : '1');
        TestDisplay(id);
      }
      Thread.Sleep(5);
      ++i;
    }

    var botProxyRequests = container.GetInstance<AbstractBotMessageSender>();
    var bot = container.GetInstance<TelegramBot>();
    var me = await bot.GetMe();
    Console.WriteLine($"Bot name: @{me.Username}");
    var observableMessages = bot.Updates.Message
        .Catch((Exception _ex) =>
        {
          Console.WriteLine("Updates.Messages exception: {0}", _ex);
          ExceptionHandler.OnError(_ex);
          return Observable.Empty<Message>().Delay(TimeSpan.FromSeconds(1));
        })
        .Repeat()
        .Select(_message => (IRequestContext)new MessageRequestContext(_message));

    var observableCallbackPublisher = bot.Updates.CallbackQuery
        .Catch((Exception _ex) =>
        {
          Console.WriteLine("Updates.CallbackQuery exception: {0}", _ex);

          ExceptionHandler.OnError(_ex);
          return Observable.Empty<CallbackQuery>().Delay(TimeSpan.FromSeconds(1));
        })
        .Repeat()
        .Publish();

    var requestHandler = container.GetInstance<RequestHandler>();
    var constexStream = observableCallbackPublisher
        .Select(_query => new CallbackRequestContext(_query))
        .Merge(observableMessages)
        .Subscribe(requestHandler.Process, ExceptionHandler.OnError);

    var approveCallbackStream = observableCallbackPublisher
        .SelectMany(SendCallbackApprove)
        .Catch((Exception _ex) =>
        {
          Console.WriteLine("Send callback approve exception: {0}", _ex);

          ExceptionHandler.OnError(_ex);
          return Observable.Empty<bool>();
        })
        .Repeat()
        .Subscribe(_ => { }, ExceptionHandler.OnError);

    var callbackStream = observableCallbackPublisher.Connect();
    var planScheduler = container.GetInstance<PlanScheduler>();

    var schedulerTrackPublisher = planScheduler.Track()
        .Catch((Exception _ex) =>
        {
          ExceptionHandler.OnError(_ex);
          return Observable.Empty<CommandPlan.Report>();
        })
        .Repeat()
        .Publish();
    var planProcessingStream = schedulerTrackPublisher
        .Subscribe(requestHandler.ProcessPlanReport);

#pragma warning disable CS8604 // Possible null reference argument.
    var sendMessagesStream = schedulerTrackPublisher
        .Where(_report => _report.StepReport.MessageBuilder != null)
        .SelectMany(_report => botProxyRequests.ObservableSend(_report.StepReport.MessageBuilder)
          .Catch((Exception _ex) =>
            {
              Console.WriteLine("On Send Report exception: {0}", _ex);

              ExceptionHandler.OnError(_ex);
              return Observable.Empty<Message>().Delay(TimeSpan.FromSeconds(1));
            }))
        .Subscribe();
#pragma warning restore CS8604 // Possible null reference argument.
    var schedulerTrackStream = schedulerTrackPublisher.Connect();

    IDisposable subscription = new CompositeDisposable(callbackStream
    , constexStream, approveCallbackStream, planProcessingStream
    , sendMessagesStream, schedulerTrackStream);
    string? input;
    do
    {
      input = Console.ReadLine();
    } while (input != "exit");
    planScheduler.Dispose();
    planProcessingStream.Dispose();
    subscription.Dispose();

    IObservable<bool> SendCallbackApprove(CallbackQuery query)
    {
      var callbackAnswer = new AnswerCallbackQuery()
      {
        CallbackQueryId = query.Id,
      };

      return Observable.FromAsync(() => bot.AnswerCallbackQuery(callbackAnswer))
        .Catch((ApiException _ex) =>
        {
          throw new Exception($"Error on callback answer to user {query.From.Id} on request: \"{query.Data}\"", _ex);
        });
    }
  }

  private static void InstallCommands(Container container)
  {
    var installerTypes = System.Reflection.Assembly.GetExecutingAssembly()
      .GetTypes()
      .Where(_type => !_type.IsGenericType && _type.HasParent(typeof(CommandInstaller<>)));

    foreach (var type in installerTypes)
    {
      var instance = Activator.CreateInstance(type, container) as Installer;
      instance?.Install();
    }
  }
}