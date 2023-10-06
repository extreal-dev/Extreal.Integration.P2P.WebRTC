using System;
using System.Collections.Generic;
using SocketIOClient;

namespace Extreal.Integration.P2P.WebRTC
{
    /// <summary>
    /// Class that holds peer configuration.
    /// </summary>
    public class PeerConfig
    {
        /// <summary>
        /// URL of the signaling server.
        /// </summary>
        public string SignalingUrl { get; }

        /// <summary>
        /// Time to wait when P2P starting is not successful.
        /// </summary>
        public TimeSpan Timeout { get; }

        /// <summary>
        /// Socket options.
        /// </summary>
        public SocketIOOptions SocketOptions { get; }

        /// <summary>
        /// Ice server URLs.
        /// </summary>
        public List<string> IceServerUrls { get; }

        /// <summary>
        /// Creates a new peer configuration.
        /// </summary>
        /// <param name="url">URL of the signaling server</param>
        /// <param name="socketOptions">Socket options</param>
        /// <param name="iceServerUrls">Ice server URLs</param>
        /// <param name="timeout">
        /// <para>Time to wait when P2P starting is not successful</para>
        /// Default: 10 seconds
        /// </param>
        public PeerConfig(string url, SocketIOOptions socketOptions = null, List<string> iceServerUrls = null, TimeSpan timeout = default)
        {
            SignalingUrl = url;
            SocketOptions = socketOptions ?? new SocketIOOptions();
            IceServerUrls = iceServerUrls ?? new List<string>();
            Timeout = timeout == default ? TimeSpan.FromSeconds(15) : timeout;
        }
    }
}
