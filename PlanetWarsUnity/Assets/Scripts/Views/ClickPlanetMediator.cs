using System.Collections;
using UnityEngine;

public class ClickPlanetMediator : SimpleMediator
{
    [Inject]
    public ClickPlanetView View { get; set; }

    [Inject]
    public PlanetClickedSignal PlanetClickedSignal { get; set; }

    [Inject]
    public OnPlanetHoverSignal OnPlanetHoverSignal { get; set; }

    [Inject]
    public OnPlanetExitSignal OnPlanetExitSignal { get; set; }

    [Inject]
    public AppModell AppModell { get; set; }

    public override void OnRegister()
    {
        base.OnRegister();
        View.Init();
    }

    public override void OnRemove()
    {
        base.OnRemove();
    }

    private void OnMouseEnter()
    {
        OnPlanetHoverSignal.Dispatch(View.Planet);
    }

    private void OnMouseExit()
    {
        OnPlanetExitSignal.Dispatch(View.Planet);
    }

    private void OnMouseDown()
    {
        if (AppModell.CurrentlyMoving)
            return;
        OnPlanetClicked();
    }

    private void OnPlanetClicked()
    {
        AppModell.SelectedPlanet = View.Planet;
        PlanetClickedSignal.Dispatch(new PlanetClickedArgs(View.Planet));
    }
}