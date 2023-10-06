using Extreal.Core.Logging;

namespace Extreal.Integration.P2P.WebRTC
{
    public class WebGLPeerConfig : PeerConfig
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(WebGLPeerConfig));

        public WebGLPeerConfig(PeerConfig peerConfig)
            : base(peerConfig.SignalingUrl, peerConfig.SocketOptions, peerConfig.IceServerUrls, peerConfig.Timeout)
        {
        }

        public bool IsDebug => Logger.IsDebug();
    }
}
