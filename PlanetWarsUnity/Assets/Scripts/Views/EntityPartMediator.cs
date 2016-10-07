using System.Collections;
using UnityEngine;

public class EntityPartMediator : SimpleMediator
{
    [Inject]
    public EntityPartView View { get; set; }

    public override void OnRegister()
    {
        base.OnRegister();
        View.Init();
    }
}