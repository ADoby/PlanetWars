using System.Collections;
using UnityEngine;

public class MoveEntitesMediator : SimpleMediator
{
    [Inject]
    public MoveEntitesView View { get; set; }

    [Inject]
    public StartMovingEntities StartMovingEntities { get; set; }

    [Inject]
    public MoveEntitesSignal MoveEntitesSignal { get; set; }

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
        StartMovingEntities.AddListener(StartMoving);
        OnPlanetHoverSignal.AddListener(PlanetHovered);
        OnPlanetExitSignal.AddListener(PlanetExited);

        Updater.UpdateCallback -= Updated;
        Updater.UpdateCallback += Updated;

        View.Lines.gameObject.SetActive(false);
    }

    public override void OnRemove()
    {
        base.OnRemove();

        StartMovingEntities.RemoveListener(StartMoving);
    }

    private void PlanetHovered(PlanetView planet)
    {
        //Check if possible
        AppModell.CurrentHoveringPlanet = planet;
    }

    private void PlanetExited(PlanetView planet)
    {
        AppModell.CurrentHoveringPlanet = null;
    }

    private void StartMoving()
    {
        if (AppModell.SelectedPlanet == null)
            return;
        enabled = true;
        View.Lines.gameObject.SetActive(true);
        SetColor(Color.red);
        AppModell.CurrentlyMoving = true;
    }

    private void SetColor(Color color)
    {
        View.Lines.SetColors(color, color);
    }

    private void StopMoving()
    {
        enabled = false;
        View.Lines.gameObject.SetActive(false);
        AppModell.CurrentlyMoving = false;
    }

    private void Updated(float deltaTime)
    {
        if (!enabled)
            return;
        if (Input.GetMouseButtonDown(1))
        {
            StopMoving();
            return;
        }
        if (AppModell.SelectedPlanet == null)
        {
            StopMoving();
            return;
        }

        Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        position.z = 0;

        View.Lines.SetPosition(0, AppModell.SelectedPlanet.Target.position);
        View.Lines.SetPosition(1, position);

        if (AppModell.CurrentHoveringPlanet != null && AppModell.SelectedPlanet != AppModell.CurrentHoveringPlanet)
        {
            SetColor(Color.green);
            if (Input.GetMouseButtonDown(0))
            {
                MoveEntitesSignal.Dispatch(new MoveEntitiesArgs(AppModell.SelectedPlanet, AppModell.CurrentHoveringPlanet));
                StopMoving();
            }
        }
        else
        {
            SetColor(Color.red);
        }
    }
}