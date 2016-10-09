using strange.extensions.mediation.impl;
using UnityEngine;

public class SimpleView : View
{
    public virtual void Init()
    {
    }

    private Transform cachedTransform;

    public new Transform transform
    {
        get
        {
            if (cachedTransform == null)
                cachedTransform = base.transform;
            return cachedTransform;
        }
    }

    private GameObject cachedGameObject;

    public new GameObject gameObject
    {
        get
        {
            if (cachedGameObject == null)
                cachedGameObject = base.gameObject;
            return cachedGameObject;
        }
    }

    private Rigidbody rigid;

    public Rigidbody Rigid
    {
        get
        {
            if (rigid == null)
                rigid = GetComponent<Rigidbody>();
            return rigid;
        }
        set
        {
            rigid = value;
        }
    }
}