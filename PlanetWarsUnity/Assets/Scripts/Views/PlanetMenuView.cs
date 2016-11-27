using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlanetMenuView : SimpleMVCSBehaviour
{
    [Inject]
    public PlanetClickedSignal PlanetClickedSignal { get; set; }

    [Inject]
    public UpdateUIPositionSignal UpdateUIPositionSignal { get; set; }

    [Inject]
    public MouseClickSignal MouseClickSignal { get; set; }

    [Inject]
    public AppModell AppModell { get; set; }

    private Vector3 relativPos;

    private Vector3 WorldPos
    {
        get
        {
            return Camera.main.WorldToScreenPoint(relativPos);
        }
    }

    public override void BindToContext(SimpleContext context)
    {
        base.BindToContext(context);
        Bind(this);
    }

    public override void OnRegister()
    {
        base.OnRegister();
        //PlanetClickedSignal.AddListener(OnPlanetClicked);
        MouseClickSignal.AddListener(OnMouseClicked);

        UpdateUIPositionSignal.AddListener(OnUpdateUIPosition);

        CloseMenu();
    }

    public override void OnRemove()
    {
        base.OnRemove();
        //PlanetClickedSignal.RemoveListener(OnPlanetClicked);
        UpdateUIPositionSignal.RemoveListener(OnUpdateUIPosition);
    }

    public void CloseMenu()
    {
        AppModell.PlanetMenuShown = false;
        gameObject.SetActive(false);
    }

    private void OnMouseClicked(int button)
    {
        if (AppModell.CurrentHoveringPlanet == null)
        {
            CloseMenu();
            return;
        }
        if (AppModell.CurrentlyMoving && AppModell.PlanetMenuShown)
        {
            return;
        }
        if (AppModell.CurrentHoveringPlanet.Player != 1)
        {
            return;
        }

        AppModell.PlanetMenuShown = true;

        relativPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = WorldPos;

        gameObject.SetActive(true);
    }

    private void OnUpdateUIPosition()
    {
        transform.position = WorldPos;
    }
}