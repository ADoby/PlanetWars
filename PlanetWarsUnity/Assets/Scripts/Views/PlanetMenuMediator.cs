using System.Collections;
using UnityEngine;

public class PlanetMenuMediator : SimpleMediator
{
    [Inject]
    public PlanetMenuView View { get; set; }

    [Inject]
    public PlanetClickedSignal PlanetClickedSignal { get; set; }

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
        gameObject.SetActive(false);
        View.FullScreenCloseButton.SetActive(false);
    }

    private void OnPlanetClicked(PlanetClickedArgs args)
    {
        transform.position = Camera.main.WorldToScreenPoint(args.Target.Target.position);
        gameObject.SetActive(true);
        View.FullScreenCloseButton.SetActive(true);
    }
}