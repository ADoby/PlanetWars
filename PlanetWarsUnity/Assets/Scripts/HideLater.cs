using System.Collections;
using UnityEngine;

public class HideLater : MonoBehaviour
{
    private float timer = 0f;

    private void Awake()
    {
        target = gameObject;
        Updater.UpdateCallback -= Updated;
        Updater.UpdateCallback += Updated;
    }

    private void OnEnable()
    {
        timer = 0f;
    }

    public float ShowTime = 1f;

    private GameObject target;

    private void Updated(float deltaTime)
    {
        timer += deltaTime;
        if (timer >= ShowTime)
        {
            target.SetActive(false);
        }
    }
}