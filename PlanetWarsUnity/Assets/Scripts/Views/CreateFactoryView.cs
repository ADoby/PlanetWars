using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CreateFactoryView : SimpleView
{
    public VoidSignal CreateFactorySignal;

    public override void Init()
    {
        base.Init();
        CreateFactorySignal = new VoidSignal();
    }

    public void CreateFactory()
    {
        CreateFactorySignal.Dispatch();
    }
}