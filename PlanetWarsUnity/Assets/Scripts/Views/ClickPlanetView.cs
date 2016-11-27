using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ClickPlanetView : SimpleMVCSBehaviour
{
    public PlanetView Planet;

    [Inject]
    public PlanetClickedSignal PlanetClickedSignal { get; set; }

    [Inject]
    public OnPlanetHoverSignal OnPlanetHoverSignal { get; set; }

    [Inject]
    public OnPlanetExitSignal OnPlanetExitSignal { get; set; }

    [Inject]
    public AppModell AppModell { get; set; }

    public override void BindToContext(SimpleContext context)
    {
        base.BindToContext(context);
        Bind(this);
    }

    public override void OnRegister()
    {
        base.OnRegister();
    }

    public override void OnRemove()
    {
        base.OnRemove();
    }

    private void OnMouseEnter()
    {
        OnPlanetHoverSignal.Dispatch(Planet);
    }

    private void OnMouseExit()
    {
        OnPlanetExitSignal.Dispatch(Planet);
    }

    private void OnMouseUpAsButton()
    {
        if (AppModell.CurrentlyMoving)
            return;
        OnPlanetClicked();
    }

    private void OnPlanetClicked()
    {
        AppModell.SelectedPlanet = Planet;
        PlanetClickedSignal.Dispatch(new PlanetClickedArgs(Planet));
    }
}