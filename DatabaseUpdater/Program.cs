using Hedgey.Extensions;
using Hedgey.Sirena.Bot.Operations;
using SimpleInjector;
using System.Reactive.Linq;

namespace DatabaseUpdater
{
  static internal class Program
  {
    private static bool complete =false;

    static void Main(string[] args)
    {
      Container container = new Container();
      var installer =  new CoreInstaller(container);
      installer.Install();
      container.Verify();
      // Вызов метода обновления базы данных
      
      IIDGenerator idGen = container.GetInstance<IIDGenerator>();
      var operation = container.GetInstance<IUpdateSirenaOperation>();
      var updateStream = UpdateSirenaWithBlendedflakeID(idGen,operation);
      while(!complete);
      Console.WriteLine("Finished");
    }
    static IDisposable UpdateSirenaWithBlendedflakeID(IIDGenerator idGen, IUpdateSirenaOperation operation)
    {
      var id = idGen.Get();
      var updateOperation = operation.UpdateDefault(id);

      return updateOperation.Expand(isUpdateSuccess =>
          {
            if (isUpdateSuccess)
            {
              id = idGen.Get();
              return operation.UpdateDefault(id);
            }
            return Observable.Empty<bool>();
          })
       .Where(x => x)
       .Count()
       .Subscribe(x => 
        {
          Console.WriteLine($"Обновлено: {x} записей");
          complete = true;
       });
    }
  }
}
