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

    public float Health = 100f;

    public VoidSignal DiedSignal;
    public List<EntityView> Enemies;

    public override void Init()
    {
        base.Init();
        DiedSignal = new VoidSignal();
        Enemies = new List<EntityView>();
    }

    public bool IsDead
    {
        get
        {
            return Health <= 0f;
        }
    }

    public void DoDamage(float damage)
    {
        if (IsDead)
            return;
        if (damage < 0)
            return;
        Health = Mathf.Max(Health - damage, 0f);
        if (IsDead)
            DiedSignal.Dispatch();
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

    private EntityView enemy;

    public void OnTriggerEnter(Collider other)
    {
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
}