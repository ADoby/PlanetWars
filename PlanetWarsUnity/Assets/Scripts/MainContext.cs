using System.Collections;
using UnityEngine;

public class MainContext : SimpleContext
{
    public MainContext(MonoBehaviour contextView) : base(contextView)
    {
    }

    protected override void BindContext()
    {
        base.BindContext();

        Bind<AppModell>(true, false);

        Bind<PlanetConnectionAddedSignal>(true, false);
        Bind<CreateFactorySignal>(true, false);
        Bind<PlanetClickedSignal>(true, false);
        Bind<SendEntityToPlanetSignal>(true, false);
        Bind<MoveEntitesSignal>(true, false);
        Bind<StartMovingEntities>(true, false);
        Bind<OnPlanetHoverSignal>(true, false);
        Bind<OnPlanetExitSignal>(true, false);
        Bind<EntityDiedSignal>(true, false);

        BindCommand<CreateFactorySignal, CreateFactoryCommand>();

        BindMediator<PlanetView, PlanetMediator>();
        BindMediator<BuildingView, BuildingMediator>();
        BindMediator<EntityView, EntityMediator>();
        BindMediator<EntityPartView, EntityPartMediator>();

        BindMediator<FactoryView, FactoryMediator>();
        BindMediator<CreateFactoryView, CreateFactoryMediator>();
        BindMediator<ClickPlanetView, ClickPlanetMediator>();
        BindMediator<PlanetMenuView, PlanetMenuMediator>();
        BindMediator<CameraView, CameraMediator>();
        BindMediator<MoveEntitesView, MoveEntitesMediator>();
    }
}