using Hedgey.Localization;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;

namespace Hedgey.Sirena.Bot;

public class DeleteSirenaPlanFactory : IFactory<IRequestContext, CommandPlan>
{
  private readonly IFindSirenaOperation findSirenaOperation;
  private readonly IGetUserRelatedSirenas getUserSirenasOperation;
  private readonly IDeleteSirenaOperation deleteSirenaOperation;
  private readonly ILocalizationProvider localizationProvider;

  public DeleteSirenaPlanFactory(IFindSirenaOperation findSirenaOperation
  , IGetUserRelatedSirenas getUserSirenasOperation
  , IDeleteSirenaOperation deleteSirenaOperation,
ILocalizationProvider localizationProvider)
  {
    this.findSirenaOperation = findSirenaOperation;
    this.getUserSirenasOperation = getUserSirenasOperation;
    this.deleteSirenaOperation = deleteSirenaOperation;
    this.localizationProvider = localizationProvider;
  }

  public CommandPlan Create(IRequestContext context)
  {
    Container<IRequestContext> contextContainer = new(context);
    NullableContainer<SirenRepresentation> sirenaContainer = new ();
    CommandStep[] steps = [
      new FindRemoveSirenaStep(contextContainer,sirenaContainer,findSirenaOperation, getUserSirenasOperation, localizationProvider),
      new ConfirmationRemoveSirenaStep(contextContainer,sirenaContainer, localizationProvider),
      new DeleteConcretteSirenaStep(contextContainer,sirenaContainer,deleteSirenaOperation, localizationProvider),
    ];
    return new(steps, contextContainer);
  }
}