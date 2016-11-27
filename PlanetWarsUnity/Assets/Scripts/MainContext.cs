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
        Bind<OnPlanetHoverSignal>(true, false);
        Bind<OnPlanetExitSignal>(true, false);
        Bind<PlanetClickedSignal>(true, false);
        Bind<SendEntityToPlanetSignal>(true, false);
        Bind<MoveEntitesSignal>(true, false);
        Bind<StartMovingEntities>(true, false);
        Bind<EntityDiedSignal>(true, false);
        Bind<UpdateUIPositionSignal>(true, false);
        Bind<MouseDownSignal>(true, false);
        Bind<MouseUpSignal>(true, false);
        Bind<MouseClickSignal>(true, false);

        BindCommand<CreateFactorySignal, CreateFactoryCommand>();
        BindCommand<OnPlanetHoverSignal, PlanetHoverCommand>();
        BindCommand<OnPlanetExitSignal, PlanetExitCommand>();
    }
}