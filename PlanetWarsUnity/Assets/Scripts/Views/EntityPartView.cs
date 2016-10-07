using System.Collections;
using UnityEngine;

public enum PartTypes
{
    BODY,
    WEAPON,
    ENGINE
}

public class EntityPartView : SimpleView
{
    public PartTypes PartType;

    public float BuildTimer = 0f;
    public float MoveTimer = 0f;
    public Transform StartTarget;
    public Transform EndTarget;

    public override void Init()
    {
        base.Init();

        SetScale(Vector3.zero);
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