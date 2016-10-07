using System.Collections;
using UnityEngine;

public class PlanetMediator : SimpleMediator
{
    [Inject]
    public PlanetView View { get; set; }

    [Inject]
    public PlanetConnectionAddedSignal PlanetConnectionAddedSignal { get; set; }

    public override void OnRegister()
    {
        base.OnRegister();
        View.Init();
        View.PlanetAdded -= PlanetConnectionAdded;
        View.PlanetAdded += PlanetConnectionAdded;

        if (Random.value > 0.5f)
            direction = 1;
        else
            direction = -1;
    }

    private Transform Target
    {
        get
        {
            return View.Target;
        }
    }

    private int direction = 0;

    private void Update()
    {
        Target.Rotate(Vector3.forward * Time.deltaTime * View.RotationSpeed * direction);
    }

    private void PlanetConnectionAdded(PlanetView other)
    {
        PlanetConnectionAddedSignal.Dispatch(new PlanetConnectionArgs(View, other));
    }
}