using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlanetView : SimpleView
{
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
            return Target.localScale.x * 2f;
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

    public override void Init()
    {
        base.Init();
    }

    public void AddEntity(EntityView entity)
    {
        if (ConnectedEntities.Contains(entity))
            return;
        ConnectedEntities.Add(entity);
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
}