using Hedgey.Extensions;
using Hedgey.Localization;
using Hedgey.Sirena.Bot;

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

    if (command == null)
    {
      if (planIsSet)
      {
#pragma warning disable CS8604 // Possible null reference argument.
        CommandUpdateLog(context);
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
        CommandUpdateLog(context);
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

  void CommandUpdateLog(IRequestContext context)
  {
    string name = context.GetCommandName();
    var uid = context.GetUser().Id;
    var time = Shortucts.CurrentTimeLabel();

    Console.WriteLine($"{time}{uid}: update -> {name}");
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
        }; break;
      case CommandStep.Result.Canceled:
      case CommandStep.Result.Exception: planDictionary.Remove(uid); break;
      case CommandStep.Result.Wait: planDictionary[uid] = report.Plan; break;
      default:
        throw new ArgumentOutOfRangeException(nameof(report.StepReport.Result));
    }
  }
}