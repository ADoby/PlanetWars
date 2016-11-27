using System.Collections;
using UnityEngine;

public class SimpleTestBehaviour : SimpleMVCSBehaviour
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
        Debug.Log("Start Test", this);

        GameObject go = new GameObject("Child");
        go.transform.SetParent(transform);
        var script = go.AddComponent<SimpleTest2Behaviour>();
        script.BubbleFast();
    }

    private void OnButtonClicked()
    {
        Debug.Log("Button Clicked");
    }
}