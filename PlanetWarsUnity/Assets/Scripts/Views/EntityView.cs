using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityStates
{
    IDLE_AROUND_PLANET,
    MOVE_TO_PLANET
}

public class EntityView : SimpleMVCSBehaviour
{
    [Inject]
    public SendEntityToPlanetSignal SendEntityToPlanetSignal { get; set; }

    [Inject]
    public MoveEntitesSignal MoveEntitesSignal { get; set; }

    [Inject]
    public EntityDiedSignal EntityDiedSignal { get; set; }

    public int Player = 0;

    public PlanetView ConnectedPlanet;

    public List<EntityPartView> Parts;

    public float Speed = 5f;
    public float RotateSpeed = 5f;

    public float CollisionIgnoreTime = 0.5f;
    public float ForceToRadius = 1f;

    public int UpdateEveryXFrame = 2;
    public Collider Collider;

    public float AggroRange = 1f;
    public float AttackCooldown = 1f;

    private float health = 100f;

    public float Health
    {
        get
        {
            return health;
        }
        set
        {
            health = value;
            IsDead = health <= 0;
        }
    }

    public List<EntityView> Enemies = new List<EntityView>();

    public bool IsDead;
    public EntityStates State;

    public List<EntityPartView> Weapons;
    public float checkEnemyTimer = 0f;
    public float attackCooldownTimer = 0f;

    private Collider[] targets;
    public Vector3 direction;

    public Vector3 rotateBy;
    public int Direction = 1;

    private int counter = 0;
    private EntityView enemy;

    public override void BindToContext(SimpleContext context)
    {
        base.BindToContext(context);
        Bind(this);
    }

    public void DoDamage(float damage)
    {
        if (IsDead)
            return;
        if (damage < 0)
            return;
        Health = Mathf.Max(Health - damage, 0f);
        if (IsDead)
            Die();
    }

    private Rigidbody rigid;

    public Rigidbody Rigid
    {
        get
        {
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

    public void SetPlanet(PlanetView planet)
    {
        ConnectedPlanet = planet;
    }

    public void ResetPositionAndRotation()
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Entity"))
            return;
        enemy = other.GetComponent<EntityView>();
        if (enemy != null && !enemy.IsDead && enemy.Player != Player && !Enemies.Contains(enemy))
            Enemies.Add(enemy);
    }

    public void OnTriggerExit(Collider other)
    {
        enemy = other.GetComponent<EntityView>();
        if (enemy != null && Enemies.Contains(enemy))
            Enemies.Remove(enemy);
    }

    public override void OnRegister()
    {
        base.OnRegister();
        if (Random.value > 0.5f)
            Direction = 1;
        else
            Direction = -1;
        SendEntityToPlanetSignal.AddListener(SendToPlanet);
        MoveEntitesSignal.AddListener(OnMoveEntities);
        EntityDiedSignal.AddListener(OnEntityDied);

        Weapons = new List<EntityPartView>();
        for (int i = 0; i < Parts.Count; i++)
        {
            if (Parts[i].PartType == PartTypes.WEAPON)
            {
                Weapons.Add(Parts[i]);
            }
        }

        if (ConnectedPlanet == null)
            Collider.enabled = true;
    }

    private void Die()
    {
        if (Enemies != null)
            Enemies.Clear();
        gameObject.SetActive(false);
        EntityDiedSignal.Dispatch(this);
    }

    private void OnMoveEntities(MoveEntitiesArgs args)
    {
        if (args.From == ConnectedPlanet)
        {
            ConnectedPlanet.RemoveEntity(this);
            ConnectedPlanet = args.To;
            SetState(EntityStates.MOVE_TO_PLANET);
        }
    }

    private void OnEntityDied(EntityView view)
    {
        if (Enemies == null)
            return;
        if (Enemies.Contains(view))
            Enemies.Remove(view);
    }

    public override void OnRemove()
    {
        base.OnRemove();
        SendEntityToPlanetSignal.RemoveListener(SendToPlanet);
    }

    private void OnEnable()
    {
        Updater.FixedUpdateCallback -= FixedUpdated;
        Updater.FixedUpdateCallback += FixedUpdated;
        Updater.UpdateCallback -= Updated;
        Updater.UpdateCallback += Updated;
    }

    private void OnDisable()
    {
        Updater.FixedUpdateCallback -= FixedUpdated;
        Updater.UpdateCallback -= Updated;
    }

    private void SendToPlanet(EntityView entity, PlanetView planet)
    {
        if (entity != this)
            return;
        SetPlanet(planet);
        SetState(EntityStates.MOVE_TO_PLANET);
    }

    private void Updated(float deltaTime)
    {
        if (IsDead)
            return;
        attackCooldownTimer += deltaTime;
        if (attackCooldownTimer >= AttackCooldown)
        {
            attackCooldownTimer = 0f;

            if (Enemies != null && Enemies.Count > 0)
            {
                //Attack enemy
                for (int i = 0; i < Enemies.Count; i++)
                {
                    if (!Enemies[i].IsDead)
                    {
                        Shoot(Enemies[i]);
                        break;
                    }
                }
            }
            else
            {
                if (Player != ConnectedPlanet.Player)
                {
                    for (int i = 0; i < ConnectedPlanet.Buildings.Count; i++)
                    {
                    }
                }
            }
        }
    }

    private void Shoot(EntityView other)
    {
        if (Weapons == null)
            return;
        for (int i = 0; i < Weapons.Count; i++)
        {
            Weapons[i].Shoot(other);
        }
    }

    private void CheckForEnemies()
    {
        Enemies.Clear();
        targets = Physics.OverlapSphere(transform.position, AggroRange);
        for (int i = 0; i < targets.Length; i++)
        {
            enemy = targets[i].GetComponent<EntityView>();
            if (enemy != null && !enemy.IsDead && enemy.Player != Player)
                Enemies.Add(enemy);
        }
    }

    private void FixedUpdated(float deltaTime)
    {
        if (ConnectedPlanet == null)
            return;
        if (State == EntityStates.IDLE_AROUND_PLANET)
        {
            MoveAroundPlanet(deltaTime);
        }
        else if (State == EntityStates.MOVE_TO_PLANET)
        {
            MoveToPlanet(deltaTime);
        }
    }

    private void MoveToPlanet(float deltaTime)
    {
        if (Rigid.isKinematic)
            return;

        counter++;
        if (counter < UpdateEveryXFrame)
            return;
        counter = 0;
        deltaTime = deltaTime * UpdateEveryXFrame;

        direction = (ConnectedPlanet.Target.position - transform.position);

        if (direction.magnitude > 0.1)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion dirQ = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, dirQ, direction.magnitude * RotateSpeed * deltaTime);
        }

        Vector3 forward = transform.up * deltaTime * Speed;

        Rigid.velocity += forward;

        if (direction.magnitude < ConnectedPlanet.Radius)
        {
            SetState(EntityStates.IDLE_AROUND_PLANET);
        }
    }

    private void MoveAroundPlanet(float deltaTime)
    {
        if (Rigid.isKinematic)
            return;

        counter++;
        if (counter < UpdateEveryXFrame)
            return;
        counter = 0;
        deltaTime = deltaTime * UpdateEveryXFrame;

        Vector3 toPlanet = (transform.position - ConnectedPlanet.Target.position) * Direction;
        direction = Vector3.Cross(toPlanet, Vector3.forward);

        Vector3 middle = ConnectedPlanet.Target.position + toPlanet.normalized * ConnectedPlanet.Radius * Direction;
        middle = middle - transform.position;
        direction = direction.normalized + middle * ForceToRadius;

        //Debug.DrawRay(Target.position, direction, Color.red);

        if (direction.magnitude > 0.1)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion dirQ = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, dirQ, direction.magnitude * RotateSpeed * deltaTime);
        }

        Vector3 forward = transform.up * deltaTime * Speed;

        Rigid.velocity += forward;
    }

    private void SetState(EntityStates state)
    {
        State = state;
    }
}