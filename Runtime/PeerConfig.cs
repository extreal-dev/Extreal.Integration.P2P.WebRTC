﻿using System;
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
        /// Socket options.
        /// </summary>
        public SocketIOOptions SocketOptions { get; }

        /// <summary>
        /// Ice server configurations.
        /// </summary>
        public List<IceServerConfig> IceServerConfigs { get; }

        /// <summary>
        /// Timeout used to detect P2P start failure.
        /// </summary>
        public TimeSpan P2PTimeout { get; internal set; }

        /// <summary>
        /// Timeout used vanilla ICE.
        /// </summary>
        public TimeSpan VanillaIceTimeout { get; internal set; }

        /// <summary>
        /// Creates a new peer configuration.
        /// </summary>
        /// <param name="url">URL of the signaling server</param>
        /// <param name="socketOptions">Socket options</param>
        /// <param name="iceServerConfigs">Ice server configurations</param>
        /// <param name="p2PTimeout">
        /// <para>Timeout used to detect P2P start failure</para>
        /// Default: 15 seconds
        /// </param>
        /// <param name="vanillaIceTimeout">
        /// <para>Timeout used vanilla ICE</para>
        /// Default: 5 seconds
        /// </param>
        public PeerConfig(
            string url,
            SocketIOOptions socketOptions = null,
            List<IceServerConfig> iceServerConfigs = null,
            TimeSpan p2PTimeout = default,
            TimeSpan vanillaIceTimeout = default)
        {
            SignalingUrl = url;
            SocketOptions = socketOptions ?? new SocketIOOptions();
            IceServerConfigs = iceServerConfigs ?? new List<IceServerConfig>();
            P2PTimeout = p2PTimeout == default ? TimeSpan.FromSeconds(15) : p2PTimeout;
            VanillaIceTimeout = vanillaIceTimeout == default ? TimeSpan.FromSeconds(5) : vanillaIceTimeout;
        }
    }

    /// <summary>
    /// Class that holds ICE server configuration (such as a STUN or TURN server).
    /// </summary>
    public class IceServerConfig
    {
        /// <summary>
        /// ICE server URLs.
        /// </summary>
        public List<string> Urls { get; }

        /// <summary>
        /// Username for TURN server.
        /// </summary>
        public string Username { get; }

        /// <summary>
        /// Credential for TURN server.
        /// </summary>
        public string Credential { get; }

        /// <summary>
        /// Creates a new Ice server configuration.
        /// </summary>
        /// <param name="urls">ICE server URLs</param>
        /// <param name="username">Username for TURN server</param>
        /// <param name="credential">Credential for TURN server</param>
        public IceServerConfig(List<string> urls, string username = "", string credential = "")
        {
            Urls = urls;
            Username = username;
            Credential = credential;
        }
    }
}
