using strange.extensions.command.api;
using strange.extensions.command.impl;
using strange.extensions.context.impl;
using strange.extensions.injector.api;
using strange.extensions.injector.impl;
using strange.extensions.mediation.api;
using UnityEngine;

public class SimpleContext : MVCSContext
{
    public static MonoBehaviour Behaviour;

    public SimpleContext(MonoBehaviour contextView) : base(contextView)
    {
        Behaviour = contextView;
    }

    protected override void addCoreComponents()
    {
        base.addCoreComponents();

        // bind signal command binder
        injectionBinder.Unbind<ICommandBinder>();
        injectionBinder.Bind<ICommandBinder>().To<SignalCommandBinder>().ToSingleton();
    }

    public override void Launch()
    {
        base.Launch();
        StartSignal startSignal = injectionBinder.GetInstance<StartSignal>();
        startSignal.Dispatch();
    }

    /// <summary>
    /// Override "BindContext" instead
    /// </summary>
    protected override void mapBindings()
    {
        base.mapBindings();
        BindContext();
        AfterBinding();
    }

    protected virtual void BindContext()
    {
        Bind<StartSignal>(true, true);

        BindMediator<SimpleView, SimpleMediator>();

        BindCommand<StartSignal, SimpleCommand>().Once();
    }

    protected virtual void AfterBinding()
    {
    }

    protected virtual IMediationBinding BindMediator<View, Mediator>()
    {
        return mediationBinder.Bind<View>().To<Mediator>();
    }

    /// <summary>
    /// Working multy layer StrangeIOC binding
    /// </summary>
    /// <typeparam name="Signal"></typeparam>
    /// <typeparam name="Command"></typeparam>
    /// <param name="once"></param>
    /// <returns></returns>
    protected virtual ICommandBinding BindCommand<Signal, Command>(bool once = false, bool replace = false)
    {
        ICommandBinding binding = null;
        try
        {
            binding = commandBinder.GetBinding<Signal>();
        }
        catch (InjectionException)
        {
            //Workaround
            //Couldn't find Binding, strange throws error. why? I don't know
            //No "ContainsBinding" method yet
        }

        if (binding != null && replace)
        {
            commandBinder.Unbind(injectionBinder.GetInstance<Signal>());
            binding = null;
        }

        if (binding == null)
        {
            binding = commandBinder.Bind<Signal>().To<Command>();
        }
        else
        {
            binding = binding.To<Command>();
        }
        if (once)
            binding = binding.Once();
        return binding;
    }

    protected virtual ICommandBinding BindCommand<Command>(object key, bool once = false)
    {
        ICommandBinding binding = commandBinder.Bind(key).To<Command>();
        if (once)
            binding = binding.Once();
        return binding;
    }

    protected virtual IInjectionBinding Bind<Type>(bool singleton = false, bool crosscontext = false)
    {
        IInjectionBinding binding = injectionBinder.Bind<Type>();
        if (singleton)
            binding = binding.ToSingleton();
        if (crosscontext)
            binding = binding.CrossContext();
        return binding;
    }
}