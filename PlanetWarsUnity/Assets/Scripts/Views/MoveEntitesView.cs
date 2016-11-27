using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MoveEntitesView : SimpleMVCSBehaviour
{
    public LineRenderer Lines;

    [Inject]
    public StartMovingEntities StartMovingEntities { get; set; }

    [Inject]
    public MoveEntitesSignal MoveEntitesSignal { get; set; }

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
        StartMovingEntities.AddListener(StartMoving);

        Updater.UpdateCallback -= Updated;
        Updater.UpdateCallback += Updated;

        Lines.gameObject.SetActive(false);
    }

    public override void OnRemove()
    {
        base.OnRemove();

        StartMovingEntities.RemoveListener(StartMoving);
    }

    private void StartMoving()
    {
        if (AppModell.SelectedPlanet == null)
            return;
        enabled = true;
        Lines.gameObject.SetActive(true);
        SetColor(Color.red);
        AppModell.CurrentlyMoving = true;
    }

    private void SetColor(Color color)
    {
        Lines.SetColors(color, color);
    }

    private void StopMoving()
    {
        enabled = false;
        Lines.gameObject.SetActive(false);
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

        Lines.SetPosition(0, AppModell.SelectedPlanet.Target.position);
        Lines.SetPosition(1, position);

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