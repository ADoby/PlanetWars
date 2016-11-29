using strange.extensions.context.impl;
using UnityEngine;

public class SimpleBootstrap : ContextView
{
    public static SimpleBootstrap Instance;

    private SimpleContext simpleContext;

    public SimpleContext SimpleContext
    {
        get
        {
            if (simpleContext == null)
            {
                if (context != null && context is SimpleContext)
                    simpleContext = (SimpleContext)context;
                else
                    Debug.LogError("SimpleContext not intilized");
            }
            return simpleContext;
        }
    }

    protected virtual void Awake()
    {
        Instance = this;
        simpleContext = new SimpleContext(this);
        context = simpleContext;
    }
}