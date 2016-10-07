using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityStates
{
    IDLE_AROUND_PLANET,
    MOVE_TO_PLANET
}

public class EntityView : SimpleView
{
    public PlanetView ConnectedPlanet;

    public List<EntityPartView> Parts;

    public float Speed = 5f;
    public float RotateSpeed = 5f;

    public float CollisionIgnoreTime = 0.5f;
    public float ForceToRadius = 1f;

    public Collider2D Collider;

    public override void Init()
    {
        base.Init();
    }

    private Rigidbody2D rigid;

    public Rigidbody2D Rigid
    {
        get
        {
            if (rigid == null)
                rigid = GetComponent<Rigidbody2D>();
            return rigid;
        }
        set
        {
            rigid = value;
        }
    }

    public void SetPhysicsEnabled(bool value)
    {
        Collider.enabled = value;
        Rigid.isKinematic = !value;
    }

    public void SetParent(Transform parent, bool resetPosition = true)
    {
        transform.SetParent(parent);
        if (resetPosition)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }

    public void ResetPositionAndRotation()
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
}