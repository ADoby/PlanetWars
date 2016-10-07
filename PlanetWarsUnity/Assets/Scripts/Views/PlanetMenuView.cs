using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlanetMenuView : SimpleView
{
    public VoidSignal CloseMenuSignal;

    public GameObject FullScreenCloseButton;

    public override void Init()
    {
        base.Init();
        CloseMenuSignal = new VoidSignal();
    }

    public void CloseMenu()
    {
        CloseMenuSignal.Dispatch();
    }
}