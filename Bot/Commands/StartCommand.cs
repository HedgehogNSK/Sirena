using Hedgey.Sirena.Database;
using RxTelegram.Bot.Interface.BaseTypes;

namespace Hedgey.Sirena.Bot;

public class StartCommand : BotCustomCommmand
{  const string welcomeMessage = "Welcome to *Sirena* bot. This bot proivdes mechanism for notification people. \n Please start from /help command to familiarize yourself with the capabilities of this bot";
  private readonly FacadeMongoDBRequests requests;

  public StartCommand(string name, string description, FacadeMongoDBRequests requests)
     : base(name, description)
  {
    this.requests = requests;
  }
  public override async void Execute(Message message)
  {
    long uid = message.From.Id;
    var user = await requests.CreateUser(uid,message.Chat.Id);
    Program.messageSender.Send(uid, welcomeMessage);
  }
}