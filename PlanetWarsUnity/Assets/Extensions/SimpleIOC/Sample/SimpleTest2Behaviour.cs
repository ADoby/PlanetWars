using System.Collections;
using UnityEngine;

public class SimpleTest2Behaviour : SimpleMVCSBehaviour
{
    [Inject]
    public StartSignal StartSignal { get; set; }

    [Inject]
    public SimpleButtonSignal SimpleButtonSignal { get; set; }

    public override void OnRegister()
    {
        base.OnRegister();
        StartSignal.AddListener(OnStartSignal);
        SimpleButtonSignal.AddListener(OnButtonClicked);
    }

    public override void BindToContext(SimpleContext context)
    {
        base.BindToContext(context);
        Bind(this);
    }

    private void OnStartSignal()
    {
        Debug.Log("Start Test2", this);
    }

    private void OnButtonClicked()
    {
        Debug.Log("Button Clicked");
    }
}