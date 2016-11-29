using System.Collections;
using UnityEngine;

public class SimpleTestButton : SimpleMVCSBehaviour
{
    [Inject]
    public SimpleButtonSignal ButtonClickSignal { get; set; }

    public override void OnRegister()
    {
        base.OnRegister();
    }

    public override void BindToContext(SimpleContext context)
    {
        base.BindToContext(context);
        Bind(this);
    }

    public void OnClick()
    {
        ButtonClickSignal.Dispatch();
    }
}