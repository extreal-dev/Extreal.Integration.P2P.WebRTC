﻿using System.Diagnostics.CodeAnalysis;

namespace Extreal.Integration.P2P.WebRTC
{
    public static class PeerClientProvider
    {
        [SuppressMessage("Style", "CC0038")]
        public static PeerClient Provide(PeerConfig peerConfig)
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            return new NativePeerClient(peerConfig);
#else
            return new WebGLPeerClient(new WebGLPeerConfig(peerConfig));
#endif
        }
    }
}
