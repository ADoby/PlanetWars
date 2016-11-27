using System.Collections;
using UnityEngine;

public class AppModell
{
    public Color GetTint(int player)
    {
        if (player == 1)
        {
            return new Color(0.6f, 1f, 0.6f);
        }
        if (player == 2)
        {
            return new Color(1, 0.6f, 0.6f);
        }
        return Color.white;
    }

    public PlanetView CurrentHoveringPlanet;
    public PlanetView SelectedPlanet;

    public bool CurrentlyMoving = false;
    public bool PlanetMenuShown = false;
}