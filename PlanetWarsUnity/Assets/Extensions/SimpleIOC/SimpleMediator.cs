using strange.extensions.mediation.impl;
using System.Collections;
using UnityEngine;

public class SimpleMediator : Mediator
{
	[Inject]
	public StartSignal OnStart { get; set; }

	public override void OnRegister()
	{
		base.OnRegister();
		OnStart.AddListener(DoStart);
	}

	public override void OnRemove()
	{
		base.OnRemove();
		OnStart.RemoveListener(DoStart);
	}

	protected virtual void DoStart()
	{
	}
}