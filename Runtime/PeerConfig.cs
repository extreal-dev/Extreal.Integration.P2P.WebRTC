﻿using System.Collections.Generic;
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
        public PeerConfig(string url, SocketIOOptions socketOptions = null, List<string> iceServerUrls = null)
        {
            SignalingUrl = url;
            SocketOptions = socketOptions ?? new SocketIOOptions();
            IceServerUrls = iceServerUrls ?? new List<string>();
        }
    }
}
