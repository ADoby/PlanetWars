using System.Collections.Generic;
using UnityEngine;

public class EntityMediator : SimpleMediator
{
    [Inject]
    public EntityView View { get; set; }

    [Inject]
    public SendEntityToPlanetSignal SendEntityToPlanetSignal { get; set; }

    [Inject]
    public MoveEntitesSignal MoveEntitesSignal { get; set; }

    [Inject]
    public EntityDiedSignal EntityDiedSignal { get; set; }

    public EntityStates State;

    public List<EntityPartView> Weapons;
    public float checkEnemyTimer = 0f;
    public float attackCooldownTimer = 0f;

    private Collider[] targets;
    private EntityView enemy;
    public Vector3 direction;

    public Vector3 rotateBy;
    public int Direction = 1;

    private int counter = 0;

    public List<EntityView> Enemies
    {
        get
        {
            return View.Enemies;
        }
        set
        {
            View.Enemies = value;
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
        View.DiedSignal.AddListener(Die);
        SendEntityToPlanetSignal.AddListener(SendToPlanet);
        MoveEntitesSignal.AddListener(OnMoveEntities);
        EntityDiedSignal.AddListener(OnEntityDied);

        Weapons = new List<EntityPartView>();
        for (int i = 0; i < View.Parts.Count; i++)
        {
            if (View.Parts[i].PartType == PartTypes.WEAPON)
            {
                Weapons.Add(View.Parts[i]);
            }
        }

        if (View.ConnectedPlanet == null)
            View.Collider.enabled = true;
    }

    private void Die()
    {
        if (Enemies != null)
            Enemies.Clear();
        gameObject.SetActive(false);
        EntityDiedSignal.Dispatch(View);
    }

    private void OnMoveEntities(MoveEntitiesArgs args)
    {
        if (args.From == View.ConnectedPlanet)
        {
            View.ConnectedPlanet.RemoveEntity(View);
            View.ConnectedPlanet = args.To;
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
        if (entity != this.View)
            return;
        View.ConnectedPlanet = planet;
        SetState(EntityStates.MOVE_TO_PLANET);
    }

    private void Updated(float deltaTime)
    {
        if (View.IsDead)
            return;
        /*
        checkEnemyTimer += deltaTime;
        if (checkEnemyTimer >= 2f)
        {
            CheckForEnemies();
            checkEnemyTimer = 0;
        }*/
        attackCooldownTimer += deltaTime;
        if (attackCooldownTimer >= View.AttackCooldown)
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
        targets = Physics.OverlapSphere(transform.position, View.AggroRange);
        for (int i = 0; i < targets.Length; i++)
        {
            enemy = targets[i].GetComponent<EntityView>();
            if (enemy != null && !enemy.IsDead && enemy.Player != View.Player)
                Enemies.Add(enemy);
        }
    }

    private void FixedUpdated(float deltaTime)
    {
        if (View.ConnectedPlanet == null)
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
        if (View.Rigid.isKinematic)
            return;

        counter++;
        if (counter < View.UpdateEveryXFrame)
            return;
        counter = 0;
        deltaTime = deltaTime * View.UpdateEveryXFrame;

        direction = (View.ConnectedPlanet.Target.position - transform.position);

        if (direction.magnitude > 0.1)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion dirQ = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, dirQ, direction.magnitude * View.RotateSpeed * deltaTime);
        }

        Vector3 forward = transform.up * deltaTime * View.Speed;

        View.Rigid.velocity += forward;

        if (direction.magnitude < View.ConnectedPlanet.Radius)
        {
            SetState(EntityStates.IDLE_AROUND_PLANET);
        }
    }

    private void MoveAroundPlanet(float deltaTime)
    {
        if (View.Rigid.isKinematic)
            return;

        counter++;
        if (counter < View.UpdateEveryXFrame)
            return;
        counter = 0;
        deltaTime = deltaTime * View.UpdateEveryXFrame;

        Vector3 toPlanet = (transform.position - View.ConnectedPlanet.Target.position) * Direction;
        direction = Vector3.Cross(toPlanet, Vector3.forward);

        Vector3 middle = View.ConnectedPlanet.Target.position + toPlanet.normalized * View.ConnectedPlanet.Radius * Direction;
        middle = middle - transform.position;
        direction = direction.normalized + middle * View.ForceToRadius;

        //Debug.DrawRay(Target.position, direction, Color.red);

        if (direction.magnitude > 0.1)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion dirQ = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, dirQ, direction.magnitude * View.RotateSpeed * deltaTime);
        }

        Vector3 forward = transform.up * deltaTime * View.Speed;

        View.Rigid.velocity += forward;
    }

    private void SetState(EntityStates state)
    {
        State = state;
    }
}