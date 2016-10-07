using System.Collections;
using UnityEngine;

public class EntityMediator : SimpleMediator
{
    [Inject]
    public EntityView View { get; set; }

    [Inject]
    public SendEntityToPlanetSignal SendEntityToPlanetSignal { get; set; }

    public EntityStates State;

    private Transform target;

    private Transform Target
    {
        get
        {
            if (target == null)
                target = GetComponent<Transform>();
            return target;
        }
    }

    public override void OnRegister()
    {
        base.OnRegister();
        if (Random.value > 0.5f)
            Direction = 1;
        else
            Direction = -1;
        View.Init();
        SendEntityToPlanetSignal.AddListener(SendToPlanet);
    }

    public override void OnRemove()
    {
        base.OnRemove();
        SendEntityToPlanetSignal.RemoveListener(SendToPlanet);
    }

    private void SendToPlanet(EntityView entity, PlanetView planet)
    {
        if (entity != this.View)
            return;
        View.ConnectedPlanet = planet;
        SetState(EntityStates.MOVE_TO_PLANET);
    }

    private void Update()
    {
        collisionIgnoreTimer += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (State == EntityStates.IDLE_AROUND_PLANET)
        {
            MoveAroundPlanet();
        }
        else if (State == EntityStates.MOVE_TO_PLANET)
        {
            MoveToPlanet();
        }
    }

    private void MoveToPlanet()
    {
    }

    public Vector3 direction;

    public Vector3 rotateBy;
    public int Direction = 1;

    private void MoveAroundPlanet()
    {
        if (View.Rigid.isKinematic)
            return;
        Vector3 toPlanet = (Target.position - View.ConnectedPlanet.Target.position) * Direction;
        direction = Vector3.Cross(toPlanet, Vector3.forward);

        Vector3 middle = View.ConnectedPlanet.Target.position + toPlanet.normalized * View.ConnectedPlanet.Radius * Direction;
        middle = middle - Target.position;
        direction = direction.normalized + middle * View.ForceToRadius;

        if (direction.magnitude > 0.1)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion dirQ = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            Target.rotation = Quaternion.Slerp(transform.rotation, dirQ, direction.magnitude * View.RotateSpeed * Time.fixedDeltaTime);
        }

        Vector2 forward = Target.up * Time.fixedDeltaTime * View.Speed;

        View.Rigid.velocity += forward;
    }

    private float collisionIgnoreTimer = 0;
    /*
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collisionIgnoreTimer > View.CollisionIgnoreTime)
        {
            Direction *= -1;
            collisionIgnoreTimer = 0f;
        }
    }*/

    private void SetState(EntityStates state)
    {
        State = state;
    }
}