using System.Collections;
using UnityEngine;

public class CreateFactoryMediator : SimpleMediator
{
    [Inject]
    public CreateFactoryView View { get; set; }

    [Inject]
    public CreateFactorySignal CreateFactorySignal { get; set; }

    public override void OnRegister()
    {
        base.OnRegister();
        View.Init();
        View.CreateFactorySignal.AddListener(OnShouldCreateFactory);
    }

    public override void OnRemove()
    {
        base.OnRemove();
        View.CreateFactorySignal.RemoveListener(OnShouldCreateFactory);
    }

    private void OnShouldCreateFactory()
    {
        CreateFactorySignal.Dispatch();
    }
}