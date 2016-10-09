using strange.extensions.signal.impl;

public struct PlanetConnectionArgs
{
    public PlanetView From;
    public PlanetView To;

    public PlanetConnectionArgs(PlanetView from, PlanetView to)
    {
        From = from;
        To = to;
    }
}

public class PlanetConnectionAddedSignal : Signal<PlanetConnectionArgs> { };

public class SendEntityToPlanetSignal : Signal<EntityView, PlanetView> { };

public class CreateFactorySignal : Signal { };

public struct PlanetClickedArgs
{
    public PlanetView Target;

    public PlanetClickedArgs(PlanetView target)
    {
        Target = target;
    }
}

public class PlanetClickedSignal : Signal<PlanetClickedArgs> { };

public struct MoveEntitiesArgs
{
    public PlanetView From;
    public PlanetView To;

    public MoveEntitiesArgs(PlanetView from, PlanetView to)
    {
        From = from;
        To = to;
    }
}

public class MoveEntitesSignal : Signal<MoveEntitiesArgs> { };

public class OnPlanetHoverSignal : Signal<PlanetView> { };

public class OnPlanetExitSignal : Signal<PlanetView> { };

public class StartMovingEntities : Signal { };

public class EntityDiedSignal : Signal<EntityView> { };