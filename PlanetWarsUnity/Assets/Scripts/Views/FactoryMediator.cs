using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryMediator : SimpleMediator
{
    [Inject]
    public FactoryView View { get; set; }

    public override void OnRegister()
    {
        base.OnRegister();
        View.Init();
    }

    private bool CreatingShip = false;
    private bool BuildingShip = false;
    private EntityView entity;
    private Queue<EntityPartView> Bodies;
    private EntityPartView currentBody;
    private Queue<EntityPartView> Weapons;
    private EntityPartView currentWeapon;
    private Queue<EntityPartView> Engines;
    private EntityPartView currentEngine;

    private void Update()
    {
        if (!View.Built)
            return;
        if (!CreatingShip)
        {
            CreatingShip = true;
            StartCoroutine(StartCreatingShip());
            return;
        }
        if (!BuildingShip)
            return;

        if (UpdatePart(currentBody))
        {
            currentBody = null;
            if (Bodies != null && Bodies.Count > 0)
                currentBody = Bodies.Dequeue();
        }
        if (UpdatePart(currentWeapon))
        {
            currentWeapon = null;
            if (Weapons != null && Weapons.Count > 0)
                currentWeapon = Weapons.Dequeue();
        }
        if (UpdatePart(currentEngine))
        {
            currentEngine = null;
            if (Engines != null && Engines.Count > 0)
                currentEngine = Engines.Dequeue();
        }
        if (currentBody == null && currentWeapon == null && currentEngine == null)
        {
            //Finished
            entity.SetParent(null, false);
            entity.SetPhysicsEnabled(true);
            entity = null;
            CreatingShip = false;
            BuildingShip = false;
        }
    }

    public float BuildTime
    {
        get
        {
            return View.BuildTime;
        }
    }

    public float MoveTime
    {
        get
        {
            return View.MoveTime;
        }
    }

    private bool UpdatePart(EntityPartView part)
    {
        if (part == null)
            return true;
        if (BuildPart(part))
        {
            if (MovePart(part))
            {
                return true;
            }
        }
        return false;
    }

    private bool BuildPart(EntityPartView part)
    {
        if (part == null)
            return true;
        part.BuildTimer = Mathf.Min(part.BuildTimer + Time.deltaTime, BuildTime);

        part.SetScale(Vector3.one * View.BuildCurve.Evaluate(part.BuildTimer / BuildTime));

        return part.BuildTimer >= BuildTime;
    }

    private bool MovePart(EntityPartView part)
    {
        if (part == null)
            return true;
        part.MoveTimer = Mathf.Min(part.MoveTimer + Time.deltaTime, MoveTime);

        part.SetPosition(Vector3.Lerp(part.StartTarget.position, part.EndTarget.position, View.MoveCurve.Evaluate(part.MoveTimer / MoveTime)));

        return part.MoveTimer >= MoveTime;
    }

    private IEnumerator StartCreatingShip()
    {
        var loading = Resources.LoadAsync<GameObject>("Ship");
        yield return loading;

        GameObject go = Object.Instantiate(loading.asset) as GameObject;

        go.transform.localScale = Vector3.one;

        entity = go.GetComponent<EntityView>();
        entity.SetParent(View.ShipPosition);
        entity.ConnectedPlanet = View.ConnectedPlanet;

        yield return null;
        entity.ResetPositionAndRotation();

        Bodies = new Queue<EntityPartView>();
        Weapons = new Queue<EntityPartView>();
        Engines = new Queue<EntityPartView>();

        for (int i = 0; i < entity.Parts.Count; i++)
        {
            entity.Parts[i].BuildTimer = 0;
            entity.Parts[i].MoveTimer = 0;

            if (entity.Parts[i].PartType == PartTypes.BODY)
            {
                entity.Parts[i].StartTarget = View.BodyFactory;
                Bodies.Enqueue(entity.Parts[i]);
            }
            if (entity.Parts[i].PartType == PartTypes.WEAPON)
            {
                entity.Parts[i].StartTarget = View.WeaponFactory;
                Weapons.Enqueue(entity.Parts[i]);
            }
            if (entity.Parts[i].PartType == PartTypes.ENGINE)
            {
                entity.Parts[i].StartTarget = View.EngineFactory;
                Engines.Enqueue(entity.Parts[i]);
            }

            entity.Parts[i].SetPosition(entity.Parts[i].StartTarget.position);
            entity.Parts[i].SetScale(Vector3.zero);
        }

        currentBody = Bodies.Dequeue();
        currentWeapon = Weapons.Dequeue();
        currentEngine = Engines.Dequeue();

        BuildingShip = true;
    }
}