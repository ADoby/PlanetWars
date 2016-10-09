using System.Collections;
using UnityEngine;

public class CreateFactoryMediator : SimpleMediator
{
    [Inject]
    public CreateFactoryView View { get; set; }

    [Inject]
    public CreateFactorySignal CreateFactorySignal { get; set; }

    [Inject]
    public StartMovingEntities StartMovingEntities { get; set; }

    public override void OnRegister()
    {
        base.OnRegister();
        View.Init();
        View.CreateFactorySignal.AddListener(OnShouldCreateFactory);
        View.MoveEntitesSignal.AddListener(OnShouldMoveEntities);
    }

    public override void OnRemove()
    {
        base.OnRemove();
        View.CreateFactorySignal.RemoveListener(OnShouldCreateFactory);
        View.MoveEntitesSignal.RemoveListener(OnShouldMoveEntities);
    }

    private void OnShouldCreateFactory()
    {
        CreateFactorySignal.Dispatch();
    }

    private void OnShouldMoveEntities()
    {
        StartMovingEntities.Dispatch();
    }
}