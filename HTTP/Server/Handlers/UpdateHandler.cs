using Hedgey.Extensions.NetCoreServer;
using NetCoreServer;
using RxTelegram.Bot.Interface.Setup;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Hedgey.Sirena.HTTP.Server;
public class UpdateHandler : IHTTPRequestHandler
{
  private readonly ISubject<Update> observableUpdates;
  public IObservable<Update> Update => observableUpdates.AsObservable();

  public UpdateHandler(ISubject<Update> observableUpdates)
  {
    this.observableUpdates = observableUpdates;
  }

  public HttpResponse Handle(HttpRequest request)
  {
    try
    {
      Update update = Extension.ParseUpdate(request);
      observableUpdates.OnNext(update);
    }
    catch (Exception exception)
    {
      Console.WriteLine("Couldn't parse: \'" + request.Body + "\'\n" + exception);
    }

    return ResponseSuccess();
  }
  private static HttpResponse ResponseSuccess()
  {
    var response = new HttpResponse();
    response.SetBegin(200); // HTTP 200 OK
    response.SetHeader("Content-Type", "application/json"); // Необязательно
    const string successJson = "{\"status\":\"ok\"}";
    response.SetBody(successJson); // Можно оставить пустым
    return response;
  }
}