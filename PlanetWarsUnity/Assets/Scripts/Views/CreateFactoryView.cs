using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CreateFactoryView : SimpleMVCSBehaviour
{
    public override void BindToContext(SimpleContext context)
    {
        base.BindToContext(context);
        Bind(this);
    }

    public void CreateFactory()
    {
        OnShouldCreateFactory();
    }

    public void MoveEntites()
    {
        OnShouldMoveEntities();
    }

    [Inject]
    public CreateFactorySignal CreateFactorySignal { get; set; }

    [Inject]
    public StartMovingEntities StartMovingEntities { get; set; }

    public override void OnRegister()
    {
        base.OnRegister();
    }

    public override void OnRemove()
    {
        base.OnRemove();
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