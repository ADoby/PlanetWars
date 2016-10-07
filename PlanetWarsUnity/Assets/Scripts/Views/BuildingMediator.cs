using System.Collections;
using UnityEngine;

public class BuildingMediator : SimpleMediator
{
    [Inject]
    public BuildingView View { get; set; }

    public override void OnRegister()
    {
        base.OnRegister();
        View.Init();
    }
}