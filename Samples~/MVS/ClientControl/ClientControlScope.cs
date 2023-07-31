using System;
using System.Collections.Generic;
using SocketIOClient;
using VContainer;
using VContainer.Unity;

namespace Extreal.Integration.P2P.WebRTC.MVS.ClientControl
{
    public class ClientControlScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            var peerConfig = new PeerConfig(
                "http://127.0.0.1:3010",
                new SocketIOOptions
                {
                    ConnectionTimeout = TimeSpan.FromSeconds(3)
                },
                new List<string>
                {
                    "stun:stun.l.google.com:19302",
                    "stun:stun1.l.google.com:19302",
                    "stun:stun2.l.google.com:19302",
                    "stun:stun3.l.google.com:19302",
                    "stun:stun4.l.google.com:19302",
                });

            var peerClient = PeerClientProvider.Provide(peerConfig);
            builder.RegisterComponent(peerClient);

#if !UNITY_WEBGL || UNITY_EDITOR
            builder.RegisterComponent<DataChannelClient>(new NativeDataChannelClient(peerClient as NativePeerClient));
#else
            builder.RegisterComponent<DataChannelClient>(new WebGLDataChannelClient());
#endif

            builder.RegisterEntryPoint<ClientControlPresenter>();
        }
    }
}
