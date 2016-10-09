using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMediator : SimpleMediator
{
    [Inject]
    public CameraView View { get; set; }

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

    public override void OnRegister()
    {
        base.OnRegister();
        View.Init();
        Updater.UpdateCallback -= Updated;
        Updater.UpdateCallback += Updated;

        cam = GetComponent<Camera>();
        wantedSize = cam.orthographicSize;
    }

    private float wantedSize = 0f;

    private Vector3 lastMousePosition;
    private Vector3 mouseDelta;

    private void Updated(float deltaTime)
    {
        float scroll = -Input.GetAxis("Mouse ScrollWheel");
        float dragX = Input.GetAxis("Horizontal");
        float dragY = Input.GetAxis("Vertical");

        if (Input.GetMouseButtonDown(0))
        {
            lastMousePosition = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {
            mouseDelta = lastMousePosition - Input.mousePosition;
            lastMousePosition = Input.mousePosition;

            mouseDelta *= View.ZoomToDragSpeed * wantedSize;

            dragX += mouseDelta.x;
            dragY += mouseDelta.y;
        }

        Target.position += Vector3.right * dragX * View.DragSpeedX * View.DragSpeed * deltaTime;
        Target.position += Vector3.up * dragY * View.DragSpeedY * View.DragSpeed * deltaTime;

        wantedSize = Mathf.Clamp(wantedSize + scroll * View.ZoomSpeed * (wantedSize * View.DistanceBasedZoomSpeedMult) * deltaTime, View.MinZoom, View.MaxZoom);

        if (View.ZoomSmoothing <= 0)
            cam.orthographicSize = wantedSize;
        else
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, wantedSize, deltaTime * (1f / View.ZoomSmoothing));
    }
}