using System.Collections;
using UnityEngine;

public class MainBootstrap : SimpleBootstrap
{
    public MainContext mainContext;

    protected override void Awake()
    {
        mainContext = new MainContext(this);
        context = mainContext;
    }
}