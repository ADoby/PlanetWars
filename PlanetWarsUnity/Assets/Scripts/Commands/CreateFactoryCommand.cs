using System.Collections;
using UnityEngine;

public class CreateFactoryCommand : SimpleCommand
{
    [Inject]
    public AppModell AppModell { get; set; }

    public override void Execute()
    {
        if (AppModell.SelectedPlanet == null)
            return;
        if (!AppModell.SelectedPlanet.HasBuildSpace)
            return;
        WorkerFactory.CreateWork(CreateFactory());
    }

    private IEnumerator CreateFactory()
    {
        var loading = Resources.LoadAsync<GameObject>("Factory");
        yield return loading;

        GameObject go = Object.Instantiate(loading.asset) as GameObject;

        FactoryView factory = go.GetComponent<FactoryView>();
        AppModell.SelectedPlanet.AddBuilding(factory);
    }
}