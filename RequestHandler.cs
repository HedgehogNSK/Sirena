using Hedgey.Extensions;
using Hedgey.Localization;
using Hedgey.Sirena.Bot;

namespace Hedgey.Sirena;

public class RequestHandler{
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
    bool commandIsSet = botCommands.TryGetValue(commandName, out var command);
    bool planIsSet = planDictionary.TryGetValue(uid, out CommandPlan? plan);

    if (!commandIsSet)
    {
      if (planIsSet)
      {
#pragma warning disable CS8604 // Possible null reference argument.
        PushPlan(plan, context);
#pragma warning restore CS8604 // Possible null reference argument.
      }
      else
      {
        string errorNoCommand = localizationProvider.Get("miscellaneous.no_command", cultureInfo);
        messageSender.Send(uid, errorNoCommand);
      }
      return;
    }

#pragma warning disable CS8602 // Dereference of a possibly null reference.
    if (planIsSet)
    {
      if (command.Command.Equals(plan.Context.GetCommandName()))
      {
        PushPlan(plan, context);
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

    void PushPlan(CommandPlan plan, IRequestContext context)
    {
      string name = plan.Context.GetCommandName();
      var uid = context.GetUser().Id;
      Console.WriteLine($"{uid}: update -> {name}");
      plan.Update(context);
      planScheduler.Push(plan);
    }
   public void ProcessPlanReport(CommandPlan.Report report)
    {
      var uid = report.Plan.Context.GetUser().Id;
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
