using Hedgey.Extensions;
using Hedgey.Localization;
using Hedgey.Sirena.Bot;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena;

public class RequestHandler
{
  private readonly ActiveCommandsDictionary botCommands;
  private readonly IDictionary<long, CommandPlan> planDictionary;
  private readonly ILocalizationProvider localizationProvider;
  private readonly AbstractBotMessageSender messageSender;
  private readonly PlanScheduler planScheduler;

  public RequestHandler(ActiveCommandsDictionary botCommands
  , IDictionary<long, CommandPlan> planDictionary
  , ILocalizationProvider localizationProvider
  , AbstractBotMessageSender messageSender
  , PlanScheduler planScheduler)
  {
    this.botCommands = botCommands;
    this.planDictionary = planDictionary;
    this.localizationProvider = localizationProvider;
    this.messageSender = messageSender;
    this.planScheduler = planScheduler;
  }

  public void Process(IRequestContext context)
  {
    var uid = context.GetUser().Id;
    var commandName = context.GetCommandName();
    var cultureInfo = context.GetCultureInfo();
    botCommands.TryGetValue(commandName, out var command);
    bool planIsSet = planDictionary.TryGetValue(uid, out CommandPlan? plan);

#pragma warning disable CS8602 // Possible null reference argument.
    if (planIsSet && plan.IsComplete)
    {
      Console.WriteLine($"{uid} -> {command.Command} . Complete plan stil in the dictionary");
      return;
    }
#pragma warning restore CS8602 // Possible null reference argument.
    if (command == null)
    {
      if (planIsSet)
      {
#pragma warning disable CS8604 // Possible null reference argument.
        CommandUpdateLog(context,plan);
        planScheduler.Push(plan, context);
#pragma warning restore CS8604 // Possible null reference argument.
      }
      else
      {
        string errorNoCommand = localizationProvider.Get("miscellaneous.no_command", cultureInfo);
        messageSender.Send(uid, errorNoCommand);
      }
      return;
    }

    if (planIsSet)
    {
      
      if (command.Command.Equals(plan?.commandName))
      {
        CommandUpdateLog(context,plan);
        planScheduler.Push(plan, context);
        return;
      }
      else
        planDictionary.Remove(uid);
    }
    
    try
    {
      var time = Shortucts.CurrentTimeLabel();
      Console.WriteLine($"{time}{uid} -> {command.Command}");
      command.Execute(context);
    }
    catch (Exception ex)
    {
      ExceptionHandler.OnError(ex);
    }
  }

  private static void CommandUpdateLog(IRequestContext context, CommandPlan? plan)
  {
    string name = plan?.commandName ?? string.Empty;
    var uid = context.GetUser().Id;
    var time = Shortucts.CurrentTimeLabel();
    var args = context.GetArgsString();
    Console.WriteLine($"{time}{uid}: update -> {name}");
    if (!string.IsNullOrEmpty(args))
      Console.WriteLine($"Command args: '{args}'");
  }
  public void ProcessPlanReport(CommandPlan.Report report)
  {
    var uid = report.Context.GetUser().Id;
    switch (report.StepReport.Result)
    {
      case CommandStep.Result.Success:
        {
          if (report.Plan.IsComplete)
            planDictionary.Remove(uid);
        } break;
      case CommandStep.Result.Canceled:
      case CommandStep.Result.Exception: planDictionary.Remove(uid); break;
      case CommandStep.Result.Wait: planDictionary[uid] = report.Plan; break;
      default:
        throw new NotSupportedException(nameof(report.StepReport.Result));
    }
  }
}