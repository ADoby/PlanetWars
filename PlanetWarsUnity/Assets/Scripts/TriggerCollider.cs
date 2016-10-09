using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TriggerCollider : MonoBehaviour
{
    [System.Serializable]
    public class ColliderEvent : UnityEvent<Collider> { }

    public ColliderEvent TriggerEnter;
    public ColliderEvent TriggerExit;

    public void OnTriggerEnter(Collider other)
    {
        if (TriggerEnter != null)
            TriggerEnter.Invoke(other);
    }

    public void OnTriggerExit(Collider other)
    {
        if (TriggerExit != null)
            TriggerExit.Invoke(other);
    }
}