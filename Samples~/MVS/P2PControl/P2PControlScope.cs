﻿using VContainer;
using VContainer.Unity;

namespace Extreal.Integration.P2P.WebRTC.MVS.P2PControl
{
    public class P2PControlScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
            => builder.RegisterEntryPoint<P2PControlPresenter>();
    }
}
