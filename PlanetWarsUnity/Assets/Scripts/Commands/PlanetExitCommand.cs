using System.Collections;
using UnityEngine;

public class PlanetExitCommand : SimpleCommand
{
    [Inject]
    public PlanetView Planet { get; set; }

    [Inject]
    public AppModell AppModell { get; set; }

    public override void Execute()
    {
        AppModell.CurrentHoveringPlanet = null;
    }
}