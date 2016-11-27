using strange.extensions.signal.impl;
using System.Collections;
using UnityEngine;

public enum PartTypes
{
    BODY,
    WEAPON,
    ENGINE
}

public class EntityPartView : SimpleMVCSBehaviour
{
    public PartTypes PartType;

    public float BuildTimer = 0f;
    public float MoveTimer = 0f;
    public Transform StartTarget;
    public Transform EndTarget;

    [Header("Weapon")]
    public float Damage = 1;

    public LineRenderer Laser;
    private GameObject laserObject;

    public override void BindToContext(SimpleContext context)
    {
        base.BindToContext(context);
        Bind(this);
    }

    public override void OnRegister()
    {
        base.OnRegister();

        if (Laser != null)
            laserObject = Laser.gameObject;
    }

    public void Shoot(EntityView other)
    {
        //Laser.SetPosition(0, transform.position);
        Laser.SetPosition(1, transform.InverseTransformPoint(other.transform.position));

        other.DoDamage(Damage);
        laserObject.SetActive(true);
    }

    public void SetPhysicsEnabled(bool value)
    {
        //Rigid.isKinematic = !value;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void SetScale(Vector3 scale)
    {
        transform.localScale = scale;
    }
}