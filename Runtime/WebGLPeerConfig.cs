#if UNITY_WEBGL
using System.Diagnostics.CodeAnalysis;
using Extreal.Core.Logging;

namespace Extreal.Integration.P2P.WebRTC
{
    public class WebGLPeerConfig : PeerConfig
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(WebGLPeerConfig));

        [SuppressMessage("Usage", "CC0057")]
        public WebGLPeerConfig(PeerConfig peerConfig, WebGLSocketOptions webGLSocketOptions = null)
            : base(peerConfig.SignalingUrl, peerConfig.SocketOptions, peerConfig.IceServerConfigs,
                peerConfig.P2PTimeout, peerConfig.VanillaIceTimeout)
        => WebGLSocketOptions = webGLSocketOptions ?? new WebGLSocketOptions();

        public bool IsDebug => Logger.IsDebug();

        public WebGLSocketOptions WebGLSocketOptions { get; }
    }

    public class WebGLSocketOptions
    {
        public bool WithCredentials { get; }

        [SuppressMessage("Usage", "CC0057")]
        public WebGLSocketOptions(bool withCredentials = false) => WithCredentials = withCredentials;
    }
}
#endif
