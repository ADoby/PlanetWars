using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Planet
{
    public Planet(Vector3 position, float size, int player)
    {
        Position = position;
        Size = size;
        Player = player;
    }

    public Vector3 Position;

    public float Size;
    public int Player = 0;

    public float Radius
    {
        get
        {
            return Size * PlanetView.SizeToRadius;
        }
    }

    public PlanetView SpawnedPlanet;
}

public class WorldGeneratorView : SimpleMVCSBehaviour
{
    public Transform PlayerBase;
    public List<Planet> Planets;
    private Planet planet;

    public float PlayerBaseRadius = 5f;
    public float EnemyPercentage = 0.3f;
    public float EnemyMinimumDistance = 10;

    public int MaxTriesToCreatePlanet = 10;
    public float MinPlanetSize = 1;
    public float MaxPlanetSize = 5;

    public float MinSpaceBetweenPlanets = 1f;
    public float RandomPlanetSizeDifferentiation = 1f;

    public enum Generators
    {
        RECTANGULAR,
        CIRCULAR,
        DISTANCE
    }

    public Generators Generator;

    [Header("Rectangular Generator")]
    public float WorldWidth = 20;

    public float WorldHeight = 20;

    [Header("Circular Generator")]
    public float WorldRadius = 20;

    [Header("Circular Generator")]
    public float MaxDistanceToWorldCenter = 100;

    public int MaxWorkPerFrame = 5;
    private int work = 0;
    private ResourceRequest request;

    [ReadOnly]
    public int playerPlanetCount = 0;

    [ReadOnly]
    public int enemyPlanetCount = 0;

    private Vector2 PlayerPosition;

    private float WorldWidth_Half
    {
        get
        {
            return WorldWidth / 2;
        }
    }

    private float WorldHeight_Half
    {
        get
        {
            return WorldHeight / 2;
        }
    }

    public override void BindToContext(SimpleContext context)
    {
        base.BindToContext(context);
        Bind(this);
    }

    public override void OnRegister()
    {
        base.OnRegister();

        StartCoroutine(Generate());
    }

    private bool TryCreatePlanet(Vector3 position, float size, int player = 0)
    {
        if (CheckSpace(position, size))
        {
            planet = new Planet(position, size, player);
            Planets.Add(planet);

            SpawnPlanet(planet);

            return true;
        }
        return false;
    }

    private IEnumerator TryGeneratePlanets_Rect(float minSpacing, float spacing, float minSize, float size)
    {
        float max = WorldWidth_Half;
        float may = WorldHeight_Half;

        float x = -max;
        float y = -may;

        for (y = may; y > -may; y -= Random.Range(minSpacing, spacing))
        {
            for (x = -max; x < max; x += Random.Range(minSpacing, spacing))
            {
                if (TryCreatePlanet(new Vector3(x, y, 0), Random.Range(minSize, size)))
                {
                    work++;
                    if (work >= MaxWorkPerFrame)
                    {
                        work = 0;
                        yield return null;
                    }
                }
            }
        }
    }

    private float GetDistanceToZero(float x, float y)
    {
        return Mathf.Sqrt(x * x + y * y);
    }

    private float GetDistance(float x, float y, float x2, float y2)
    {
        x = x - x2;
        y = y - y2;
        return Mathf.Sqrt(x * x + y * y);
    }

    private IEnumerator TryGeneratePlanets_Circular(float minSpacing, float spacing, float minSize, float size)
    {
        float max = WorldRadius / 2f * Mathf.PI;
        float may = WorldRadius / 2f * Mathf.PI;

        float x = -max;
        float y = -may;

        int player = 0;
        for (y = may; y > -may; y -= Random.Range(minSpacing, spacing))
        {
            for (x = -max; x < max; x += Random.Range(minSpacing, spacing))
            {
                if (GetDistanceToZero(x, y) > WorldRadius)
                    continue;

                player = 0;
                if (GetDistance(x, y, PlayerPosition.x, PlayerPosition.y) > EnemyMinimumDistance)
                {
                    if (Random.value < EnemyPercentage)
                    {
                        player = 2;
                    }
                }
                if (GetDistance(x, y, PlayerPosition.x, PlayerPosition.y) < PlayerBaseRadius)
                {
                    player = 1;
                }

                if (TryCreatePlanet(new Vector3(x, y, 0), Random.Range(minSize, size), player))
                {
                    if (player == 1)
                        playerPlanetCount++;
                    if (player == 2)
                        enemyPlanetCount++;

                    work++;
                    if (work >= MaxWorkPerFrame)
                    {
                        work = 0;
                        yield return null;
                    }
                }
            }
        }
    }

    private IEnumerator GenerateRectangular()
    {
        float spacing;
        float size;
        for (float i = 1; i >= 0; i -= 1f / MaxTriesToCreatePlanet)
        {
            size = Mathf.Lerp(MinPlanetSize, MaxPlanetSize, i);
            spacing = size * PlanetView.SizeToRadius + MinSpaceBetweenPlanets;
            yield return TryGeneratePlanets_Rect(0.1f, spacing, Mathf.Max(size - RandomPlanetSizeDifferentiation, MinPlanetSize), Mathf.Min(size + RandomPlanetSizeDifferentiation, MaxPlanetSize));
        }
    }

    private IEnumerator GenerateCircular()
    {
        PlayerPosition = Random.insideUnitCircle * WorldRadius;

        PlayerBase.position = PlayerPosition;

        float spacing;
        float size;
        for (float i = 1; i >= 0; i -= 1f / MaxTriesToCreatePlanet)
        {
            size = Mathf.Lerp(MinPlanetSize, MaxPlanetSize, i);
            spacing = size * PlanetView.SizeToRadius + MinSpaceBetweenPlanets;
            yield return TryGeneratePlanets_Circular(0.1f, spacing, Mathf.Max(size - RandomPlanetSizeDifferentiation, MinPlanetSize), Mathf.Min(size + RandomPlanetSizeDifferentiation, MaxPlanetSize));
        }
    }

    private IEnumerator Generate()
    {
        request = Resources.LoadAsync<GameObject>("Planet");
        yield return request;

        if (Generator == Generators.RECTANGULAR)
        {
            yield return GenerateRectangular();
        }
        else if (Generator == Generators.CIRCULAR)
        {
            yield return GenerateCircular();
        }

        //UpdatePlanetConnections();
    }

    private GameObject go;

    private void SpawnPlanet(Planet planet)
    {
        go = Instantiate(request.asset) as GameObject;
        go.transform.localScale = planet.Size * Vector3.one;
        go.transform.position = planet.Position;

        go.name = string.Format("Planet_{0}", planet.Player);

        planet.SpawnedPlanet = go.GetComponent<PlanetView>();
        planet.SpawnedPlanet.Player = planet.Player;
        go.SendMessageUpwards("Bubble", SendMessageOptions.DontRequireReceiver);
        if (planet.Player != 0)
            planet.SpawnedPlanet.UpdateTint();
    }

    private void UpdatePlanetConnections()
    {
        List<Planet> planets;
        for (int i = 0; i < Planets.Count; i++)
        {
            planets = FindPlanets(Planets[i]);
            for (int x = 0; x < planets.Count; x++)
            {
                Planets[i].SpawnedPlanet.ConnectedPlanets.Add(planets[x].SpawnedPlanet);
            }
        }
    }

    private bool CheckSpace(Vector3 position, float size)
    {
        for (int i = 0; i < Planets.Count; i++)
        {
            if (CheckInRange(Planets[i].Position, position, Planets[i].Radius, size * PlanetView.SizeToRadius))
            {
                return false;
            }
        }

        return true;
    }

    private bool CheckInRange(Planet planet, Planet planet2)
    {
        return CheckInRange(planet.Position, planet2.Position, planet.Radius, planet2.Radius);
    }

    private bool CheckInRange(Vector3 position, Vector3 position2, float radius, float radius2)
    {
        return Vector3.Distance(position, position2) - radius - radius2 < MinSpaceBetweenPlanets;
    }

    private List<Planet> FindPlanets(Planet planet)
    {
        List<Planet> planets = new List<Planet>();
        for (int i = 0; i < Planets.Count; i++)
        {
        }
        return planets;
    }
}