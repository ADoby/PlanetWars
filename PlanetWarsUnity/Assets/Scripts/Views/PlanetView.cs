using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlanetView : SimpleMVCSBehaviour
{
    [Inject]
    public PlanetConnectionAddedSignal PlanetConnectionAddedSignal { get; set; }

    [Inject]
    public AppModell AppModell { get; set; }

    public SpriteRenderer[] TintedSprites;

    public static float SizeToRadius = 2f;
    public int Player = 0;

    public List<Transform> BuildingPositions;

    public List<PlanetView> ConnectedPlanets;
    public List<BuildingView> Buildings;

    public UnityAction<PlanetView> PlanetAdded;

    public float RotationSpeed = 1f;

    private Transform target;

    public int MaxEntities = 50;
    public int PlannedEntities = 0;

    public List<EntityView> ConnectedEntities;

    public VoidSignal UpdateTintSignal;
    private int direction = 0;

    public override void BindToContext(SimpleContext context)
    {
        base.BindToContext(context);
        Bind(this);
    }

    public bool HasEntitySpace
    {
        get
        {
            return ConnectedEntities.Count + PlannedEntities < MaxEntities;
        }
    }

    public bool HasBuildSpace
    {
        get
        {
            return Buildings.Count < BuildingPositions.Count;
        }
    }

    public float Radius
    {
        get
        {
            return Target.localScale.x * SizeToRadius;
        }
    }

    public Transform Target
    {
        get
        {
            if (target == null)
                target = GetComponent<Transform>();
            return target;
        }
    }

    public void AddEntity(EntityView entity)
    {
        if (ConnectedEntities.Contains(entity))
            return;
        ConnectedEntities.Add(entity);
    }

    public void TintSprites(Color tint)
    {
        for (int i = 0; i < TintedSprites.Length; i++)
        {
            if (TintedSprites[i] != null)
                TintedSprites[i].color = tint;
        }
    }

    public void RemoveEntity(EntityView entity)
    {
        if (!ConnectedEntities.Contains(entity))
            return;
        ConnectedEntities.Remove(entity);
    }

    public void AddConnectedPlanet(PlanetView other)
    {
        if (IsConnectedTo(other))
            return;
        if (ConnectedPlanets == null)
            ConnectedPlanets = new List<PlanetView>();
        ConnectedPlanets.Add(other);
        if (PlanetAdded != null)
            PlanetAdded.Invoke(other);
    }

    public void AddBuilding(BuildingView building)
    {
        if (BuildingPositions == null)
            return;
        if (Buildings == null)
            Buildings = new List<BuildingView>();

        if (!HasBuildSpace)
            return;
        Buildings.Add(building);
        building.ConnectedPlanet = this;
        building.SetPosition(BuildingPositions[Buildings.IndexOf(building)]);
    }

    public bool IsConnectedTo(PlanetView other)
    {
        if (ConnectedPlanets == null || ConnectedPlanets.Count == 0)
            return false;
        return ConnectedPlanets.Contains(other);
    }

    public override void OnRegister()
    {
        base.OnRegister();
        PlanetAdded -= PlanetConnectionAdded;
        PlanetAdded += PlanetConnectionAdded;

        if (Random.value > 0.5f)
            direction = 1;
        else
            direction = -1;

        Updater.UpdateCallback -= Updated;
        Updater.UpdateCallback += Updated;

        UpdateTint();
    }

    public void UpdateTint()
    {
        TintSprites(AppModell.GetTint(Player));
    }

    private void Updated(float deltaTime)
    {
        Target.Rotate(Vector3.forward * deltaTime * RotationSpeed * direction);
    }

    private void PlanetConnectionAdded(PlanetView other)
    {
        PlanetConnectionAddedSignal.Dispatch(new PlanetConnectionArgs(this, other));
    }
}