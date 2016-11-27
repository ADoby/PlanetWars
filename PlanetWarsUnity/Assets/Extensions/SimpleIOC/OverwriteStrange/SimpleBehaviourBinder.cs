/*
 * Copyright 2013 ThirdMotion, Inc.
 *
 *	Licensed under the Apache License, Version 2.0 (the "License");
 *	you may not use this file except in compliance with the License.
 *	You may obtain a copy of the License at
 *
 *		http://www.apache.org/licenses/LICENSE-2.0
 *
 *		Unless required by applicable law or agreed to in writing, software
 *		distributed under the License is distributed on an "AS IS" BASIS,
 *		WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *		See the License for the specific language governing permissions and
 *		limitations under the License.
 */

using strange.extensions.injector.api;
using strange.extensions.mediation.api;
using strange.framework.api;
using strange.framework.impl;

/**
 * @class strange.extensions.mediation.impl.MediationBinder
 *
 * Binds Views to Mediators.
 *
 * Please read strange.extensions.mediation.api.IMediationBinder
 * where I've extensively explained the purpose of View mediation
 */

using System;
using System.Collections;
using UnityEngine;

namespace strange.extensions.mediation.impl
{
    public class SimpleBehaviourBinder : Binder, IMediationBinder
    {
        [Inject]
        public IInjectionBinder injectionBinder { get; set; }

        public SimpleBehaviourBinder()
        {
        }

        public override IBinding GetRawBinding()
        {
            return new MediationBinding(resolver) as IBinding;
        }

        public void Trigger(MediationEvent evt, IView view)
        {
            Type viewType = view.GetType();
            IMediationBinding binding = GetBinding(viewType) as IMediationBinding;
            if (binding != null)
            {
                switch (evt)
                {
                    case MediationEvent.AWAKE:
                        injectViewAndChildren(view);
                        mapView(view);
                        break;

                    case MediationEvent.DESTROYED:
                        unmapView(view);
                        break;

                    default:
                        break;
                }
            }
            else if (evt == MediationEvent.AWAKE)
            {
                //Even if not mapped, Views (and their children) have potential to be injected
                injectViewAndChildren(view);
            }
        }

        /// Initialize all IViews within this view
        virtual protected void injectViewAndChildren(IView view)
        {
            MonoBehaviour mono = view as MonoBehaviour;
            Component[] views = mono.GetComponentsInChildren(typeof(IView), true) as Component[];

            for (int a = views.Length - 1; a >= 0; a--)
            {
                IView iView = views[a] as IView;
                if (iView != null)
                {
                    /*if (iView.autoRegisterWithContext && iView.registeredWithContext)
                    {
                        continue;
                    }*/
                    iView.registeredWithContext = true;

                    injectionBinder.injector.Inject(mono, false);
                    mapView(view);
                }
            }
            injectionBinder.injector.Inject(mono, false);
        }

        new public IMediationBinding Bind<T>()
        {
            return base.Bind<T>() as IMediationBinding;
        }

        public IMediationBinding BindView<T>() where T : MonoBehaviour
        {
            return base.Bind<T>() as IMediationBinding;
        }

        /// Creates and registers one or more Mediators for a specific View instance.
        /// Takes a specific View instance and a binding and, if a binding is found for that type, creates and registers a Mediator.
        virtual protected void mapView(IView view)
        {
            Type viewType = view.GetType();

            MonoBehaviour mono = view as MonoBehaviour;

            if (mono is IMediator)
                ((IMediator)mono).PreRegister();
            injectionBinder.Bind(viewType).ToValue(view).ToInject(false);
            injectionBinder.injector.Inject(mono);
            injectionBinder.Unbind(viewType);
            if (mono is IMediator)
                ((IMediator)mono).OnRegister();
        }

        /// Removes a mediator when its view is destroyed
        virtual protected void unmapView(IView view)
        {
            Type viewType = view.GetType();

            MonoBehaviour mono = view as MonoBehaviour;
            IMediator mediator = mono.GetComponent(viewType) as IMediator;
            if (mediator != null)
                mediator.OnRemove();
        }

        private void enableView(IView view)
        {
        }

        private void disableView(IView view)
        {
        }
    }
}