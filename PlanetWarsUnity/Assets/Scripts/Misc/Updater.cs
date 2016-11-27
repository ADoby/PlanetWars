using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Updater : MonoBehaviour
{
    public static UnityAction<float> UpdateCallback;
    public static UnityAction<float> LateUpdateCallback;
    public static UnityAction<float> FixedUpdateCallback;

    private void Update()
    {
        if (UpdateCallback != null)
            UpdateCallback.Invoke(Time.deltaTime);
    }

    private void LateUpdate()
    {
        if (LateUpdateCallback != null)
            LateUpdateCallback.Invoke(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (FixedUpdateCallback != null)
            FixedUpdateCallback.Invoke(Time.fixedDeltaTime);
    }
}