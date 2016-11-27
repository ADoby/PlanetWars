using System.Collections;
using UnityEngine;

public class CameraView : SimpleMVCSBehaviour
{
    public float MinZoom = 1f;
    public float MaxZoom = 10f;

    public float ZoomSpeed = 1f;
    public float DistanceBasedZoomSpeedMult = 0.1f;
    public float ZoomSmoothing = 1f;

    public float DragSmoothing = 1f;

    [Inject]
    public UpdateUIPositionSignal UpdateUIPositionSignal { get; set; }

    [Inject]
    public MouseDownSignal MouseDownSignal { get; set; }

    [Inject]
    public MouseUpSignal MouseUpSignal { get; set; }

    public Transform target;

    public Transform Target
    {
        get
        {
            if (target == null)
                target = GetComponent<Transform>();
            return target;
        }
    }

    private Camera cam;

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

        cam = GetComponent<Camera>();
        wantedSize = cam.orthographicSize;

        mouseInputPlane = new Plane(Vector3.forward, Vector3.zero);

        MouseDownSignal.AddListener(OnMouseDown);
        MouseUpSignal.AddListener(OnMouseUp);
    }

    private void OnMouseDown(int index)
    {
        if (index != 0)
            return;
        IsMouseDown = true;
        lastMousePosition = MouseWorldPosition();
    }

    private void OnMouseUp(int index)
    {
        if (index != 0)
            return;
        IsMouseDown = false;
    }

    public Vector3 MouseWorldPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private float wantedSize = 0f;

    private Vector3 lastMousePosition;
    private Vector3 MousePosition;
    private Vector3 mouseDelta;

    private Plane mouseInputPlane;
    private float mouseInputDistance;
    private Ray mouseInputRay;
    private Vector3 MouseMove;
    private Vector3 MouseMoveDiff;

    private float lastScroll = 0;

    private bool IsMouseDown = false;

    private void Updated(float deltaTime)
    {
        float dragX = Input.GetAxis("Horizontal");
        float dragY = Input.GetAxis("Vertical");

        if (IsMouseDown)
        {
            MousePosition = MouseWorldPosition() - Target.position;
            mouseDelta = (lastMousePosition - MousePosition);
            mouseDelta = mouseDelta - Target.position;

            MouseMove.x += mouseDelta.x;
            MouseMove.y += mouseDelta.y;
        }
        Zoom(deltaTime);

        if (DragSmoothing == 0)
        {
            Target.position += MouseMove;
            MouseMove.x = 0;
            MouseMove.y = 0;
        }
        else
        {
            MouseMoveDiff = MouseMove * DragSmoothing * deltaTime;
            Target.position += MouseMoveDiff;
            MouseMove -= MouseMoveDiff;
        }
        UpdateUIPositionSignal.Dispatch();
        lastMousePosition = MouseWorldPosition();
    }

    private void Zoom(float deltaTime)
    {
        float scroll = -Input.GetAxis("Mouse ScrollWheel");

        lastMousePosition = MouseWorldPosition();

        wantedSize = Mathf.Clamp(wantedSize + scroll * ZoomSpeed * (wantedSize * DistanceBasedZoomSpeedMult) * deltaTime, MinZoom, MaxZoom);

        if (ZoomSmoothing <= 0)
            cam.orthographicSize = wantedSize;
        else
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, wantedSize, deltaTime * (1f / ZoomSmoothing));

        MousePosition = MouseWorldPosition() - Target.position;
        mouseDelta = (lastMousePosition - MousePosition);
        mouseDelta = mouseDelta - Target.position;

        Target.position += mouseDelta;
    }
}