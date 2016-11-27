using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MouseInputView : SimpleMVCSBehaviour
{
    public float MouseMovementThreshhold = 5f;

    [Inject]
    public AppModell AppModell { get; set; }

    [Inject]
    public MouseDownSignal MouseDownSignal { get; set; }

    [Inject]
    public MouseUpSignal MouseUpSignal { get; set; }

    [Inject]
    public MouseClickSignal MouseClickSignal { get; set; }

    public List<MouseButtonInfo> MouseButtons;

    public override void BindToContext(SimpleContext context)
    {
        base.BindToContext(context);
        Bind(this);
    }

    public override void OnRegister()
    {
        base.OnRegister();
        Updater.UpdateCallback -= Updated;
        Updater.UpdateCallback += Updated;

        MouseButtons = new List<MouseButtonInfo>();
        MouseButtons.Add(new MouseButtonInfo());
        MouseButtons.Add(new MouseButtonInfo());
        MouseButtons.Add(new MouseButtonInfo());
        MouseButtons.Add(new MouseButtonInfo());
        MouseButtons.Add(new MouseButtonInfo());
        MouseButtons.Add(new MouseButtonInfo());
    }

    public override void OnRemove()
    {
        base.OnRemove();
    }

    public class MouseButtonInfo
    {
        public bool IsDown = false;
        public Vector3 OriginMousePosition;
    }

    private void Updated(float deltaTime)
    {
        for (int i = 0; i < MouseButtons.Count; i++)
        {
            if (MouseButtons[i].IsDown)
            {
                if (Input.GetMouseButtonUp(i))
                {
                    MouseButtons[i].IsDown = false;
                    MouseUpSignal.Dispatch(i);
                    //Check Distance
                    if (Vector3.Distance(Input.mousePosition, MouseButtons[i].OriginMousePosition) < MouseMovementThreshhold)
                    {
                        MouseClickSignal.Dispatch(i);
                    }
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(i))
                {
                    MouseButtons[i].OriginMousePosition = Input.mousePosition;
                    MouseButtons[i].IsDown = true;
                    MouseDownSignal.Dispatch(i);
                }
            }
        }
    }
}