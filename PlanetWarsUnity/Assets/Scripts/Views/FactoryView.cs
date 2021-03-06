﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryView : BuildingView
{
    public Transform BodyFactory;
    public Transform WeaponFactory;
    public Transform EngineFactory;
    public Transform ShipPosition;

    public float BuildTime = 1f;
    public AnimationCurve BuildCurve;
    public float MoveTime = 1f;
    public AnimationCurve MoveCurve;

    [Inject]
    public EntityDiedSignal EntityDiedSignal { get; set; }

    public override void BindToContext(SimpleContext context)
    {
        base.BindToContext(context);
        Bind(this);
    }

    public override void OnRegister()
    {
        base.OnRegister();

        EntityDiedSignal.AddListener(FreeEntity);
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
        if (!Built)
            return;
        if (!CreatingShip)
        {
            if (!ConnectedPlanet.HasEntitySpace)
                return;
            CreatingShip = true;
            ConnectedPlanet.PlannedEntities++;
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
            ConnectedPlanet.PlannedEntities--;
            ConnectedPlanet.AddEntity(entity);
            entity.SetParent(null, false);
            entity.SetPhysicsEnabled(true);
            entity = null;
            CreatingShip = false;
            BuildingShip = false;
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

        if (part.BuildTimer >= BuildTime)
            return true;

        part.SetPosition(part.StartTarget.position);

        part.BuildTimer = Mathf.Min(part.BuildTimer + Time.deltaTime, BuildTime);

        part.SetScale(Vector3.one * BuildCurve.Evaluate(part.BuildTimer / BuildTime));

        return part.BuildTimer >= BuildTime;
    }

    private bool MovePart(EntityPartView part)
    {
        if (part == null)
            return true;
        part.MoveTimer = Mathf.Min(part.MoveTimer + Time.deltaTime, MoveTime);

        part.SetPosition(Vector3.Lerp(part.StartTarget.position, part.EndTarget.position, MoveCurve.Evaluate(part.MoveTimer / MoveTime)));

        return part.MoveTimer >= MoveTime;
    }

    private static Queue<EntityView> FreeEntities = new Queue<EntityView>();

    public static void FreeEntity(EntityView entity)
    {
        if (FreeEntities.Contains(entity))
            return;
        FreeEntities.Enqueue(entity);
    }

    private IEnumerator StartCreatingShip()
    {
        if (FreeEntities.Count == 0)
        {
            var loading = Resources.LoadAsync<GameObject>("Ship");
            yield return loading;

            GameObject go = Object.Instantiate(loading.asset) as GameObject;

            entity = go.GetComponent<EntityView>();
        }
        else
        {
            entity = FreeEntities.Dequeue();
            entity.gameObject.SetActive(true);
        }

        entity.transform.localScale = Vector3.one;

        entity.SetParent(ShipPosition);
        entity.ConnectedPlanet = ConnectedPlanet;
        entity.Player = entity.ConnectedPlanet.Player;
        entity.Health = 20;

        for (int i = 0; i < entity.Parts.Count; i++)
        {
            entity.Parts[i].SetScale(Vector3.zero);
        }

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
                entity.Parts[i].StartTarget = BodyFactory;
                Bodies.Enqueue(entity.Parts[i]);
            }
            if (entity.Parts[i].PartType == PartTypes.WEAPON)
            {
                entity.Parts[i].StartTarget = WeaponFactory;
                Weapons.Enqueue(entity.Parts[i]);
            }
            if (entity.Parts[i].PartType == PartTypes.ENGINE)
            {
                entity.Parts[i].StartTarget = EngineFactory;
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