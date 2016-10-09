using System.Collections;
using UnityEngine;

public class CameraView : SimpleView
{
    public float MinZoom = 1f;
    public float MaxZoom = 10f;

    public float ZoomSpeed = 1f;
    public float DistanceBasedZoomSpeedMult = 0.1f;
    public float ZoomSmoothing = 1f;

    public float DragSpeed = 1f;
    public float DragSpeedX = 1f;
    public float DragSpeedY = 1f;

    public float ZoomToDragSpeed = 0.5f;

    public override void Init()
    {
        base.Init();
    }
}