using System.Diagnostics.CodeAnalysis;

namespace Extreal.Integration.P2P.WebRTC
{
    /// <summary>
    /// Class that provides PeerClient.
    /// </summary>
    public static class PeerClientProvider
    {
        /// <summary>
        /// Provides the PeerClient.
        /// </summary>
        /// <remarks>
        /// Creates and returns a PeerClient for Native (C#) or WebGL (JavaScript) depending on the platform.
        /// </remarks>
        /// <param name="peerConfig">Peer configuration</param>
        /// <returns>PeerClient</returns>
        [SuppressMessage("Style", "CC0038"), SuppressMessage("Style", "IDE0022")]
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
