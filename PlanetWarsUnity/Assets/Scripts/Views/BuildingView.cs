using System.Collections;
using UnityEngine;

public class BuildingView : SimpleMVCSBehaviour
{
    public bool Built = false;
    public PlanetView ConnectedPlanet;
    public AnimationCurve ScaleCurve;
    public float ScaleTime = 1f;

    public override void BindToContext(SimpleContext context)
    {
        base.BindToContext(context);
        Bind(this);
    }

    public void SetPosition(Transform target)
    {
        transform.SetParent(target);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        Show();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        StartCoroutine(ShowAnimation());
    }

    private IEnumerator ShowAnimation()
    {
        transform.localScale = Vector3.zero;
        for (float timer = 0; timer < ScaleTime; timer += Time.deltaTime)
        {
            transform.localScale = Vector3.one * ScaleCurve.Evaluate(timer / ScaleTime);
            yield return null;
        }
        Built = true;
    }
}