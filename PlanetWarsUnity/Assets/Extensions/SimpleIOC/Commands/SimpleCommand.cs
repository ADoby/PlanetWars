using strange.extensions.command.impl;
using System.Collections;
using UnityEngine;

public class SimpleCommand : Command
{
	/// <summary>
	/// Does nothing, does not need to be called
	/// </summary>
	public override void Execute()
	{
		Debug.Log("Simple Command");
	}
}