using System.Collections;
using UnityEngine;

public class PlanetMenuMediator : SimpleMediator
{
    [Inject]
    public PlanetMenuView View { get; set; }

    [Inject]
    public PlanetClickedSignal PlanetClickedSignal { get; set; }

    [Inject]
    public AppModell AppModell { get; set; }

    public override void OnRegister()
    {
        base.OnRegister();
        View.Init();
        View.CloseMenuSignal.AddListener(OnCloseMenu);
        PlanetClickedSignal.AddListener(OnPlanetClicked);

        OnCloseMenu();
    }

    public override void OnRemove()
    {
        base.OnRemove();
        View.CloseMenuSignal.RemoveListener(OnCloseMenu);
        PlanetClickedSignal.RemoveListener(OnPlanetClicked);
    }

    private void OnCloseMenu()
    {
        AppModell.PlanetMenuShown = false;
        gameObject.SetActive(false);
        View.FullScreenCloseButton.SetActive(false);
    }

    private void OnPlanetClicked(PlanetClickedArgs args)
    {
        if (AppModell.PlanetMenuShown)
            return;
        AppModell.PlanetMenuShown = true;
        transform.position = Input.mousePosition;
        gameObject.SetActive(true);
        View.FullScreenCloseButton.SetActive(true);
    }
}