using System.Collections;
using UnityEngine;

public class FactoryView : BuildingView
{
    public Transform BodyFactory;
    public Transform WeaponFactory;
    public Transform EngineFactory;
    public Transform ShipPosition;

    public float BuildTime = 1f;
    public AnimationCurve BuildCurve;
    public float MoveTime = 1f;
    public AnimationCurve MoveCurve;

    public override void Init()
    {
        base.Init();
    }
}