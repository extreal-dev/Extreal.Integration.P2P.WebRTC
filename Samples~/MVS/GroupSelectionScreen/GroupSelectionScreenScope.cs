﻿using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Extreal.Integration.P2P.WebRTC.MVS.GroupSelectionScreen
{
    public class GroupSelectionScreenScope : LifetimeScope
    {
        [SerializeField] private GroupSelectionScreenView groupSelectionScreenView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(groupSelectionScreenView);

            builder.Register<GroupProvider>(Lifetime.Singleton);

            builder.RegisterEntryPoint<GroupSelectionScreenPresenter>();
        }
    }
}
