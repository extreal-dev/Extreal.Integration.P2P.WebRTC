﻿#if UNITY_WEBGL
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using AOT;
using Cysharp.Threading.Tasks;
using Extreal.Integration.Web.Common;

namespace Extreal.Integration.P2P.WebRTC
{
    public class WebGLPeerClient : PeerClient
    {
        private static WebGLPeerClient instance;
        private StartHostResponse startHostResponse;
        private ListHostsResponse listHostsResponse;
        private CancellationTokenSource cancellation;

        public WebGLPeerClient(WebGLPeerConfig peerConfig) : base(peerConfig)
        {
            instance = this;
            cancellation = new CancellationTokenSource();
            WebGLHelper.CallAction(WithPrefix(nameof(WebGLPeerClient)), ToJson(peerConfig));
            WebGLHelper.AddCallback(WithPrefix(nameof(HandleOnStarted)), HandleOnStarted);
            WebGLHelper.AddCallback(WithPrefix(nameof(HandleOnConnectFailed)), HandleOnConnectFailed);
            WebGLHelper.AddCallback(WithPrefix(nameof(HandleOnDisconnected)), HandleOnDisconnected);
            WebGLHelper.AddCallback(WithPrefix(nameof(ReceiveStartHostResponse)), ReceiveStartHostResponse);
            WebGLHelper.AddCallback(WithPrefix(nameof(ReceiveListHostsResponse)), ReceiveListHostsResponse);
            WebGLHelper.AddCallback(WithPrefix(nameof(HandleOnUserConnecting)), HandleOnUserConnecting);
            WebGLHelper.AddCallback(WithPrefix(nameof(HandleOnUserDisconnecting)), HandleOnUserDisconnecting);
        }

        [MonoPInvokeCallback(typeof(Action<string, string>))]
        private static void HandleOnStarted(string unused1, string unused2) => instance.FireOnStarted();

        [MonoPInvokeCallback(typeof(Action<string, string>))]
        private static void HandleOnConnectFailed(string reason, string unused2) =>
            instance.FireOnConnectFailed(reason);

        [MonoPInvokeCallback(typeof(Action<string, string>))]
        private static void HandleOnDisconnected(string reason, string unused2) => instance.FireOnDisconnected(reason);

        [MonoPInvokeCallback(typeof(Action<string, string>))]
        private static void ReceiveStartHostResponse(string jsonResponse, string unused)
            => instance.startHostResponse = JsonSerializer.Deserialize<StartHostResponse>(jsonResponse);

        [MonoPInvokeCallback(typeof(Action<string, string>))]
        private static void ReceiveListHostsResponse(string jsonResponse, string unused)
            => instance.listHostsResponse = JsonSerializer.Deserialize<ListHostsResponse>(jsonResponse);

        [MonoPInvokeCallback(typeof(Action<string, string>))]
        private static void HandleOnUserConnecting(string id, string unused2) => instance.FireOnUserConnecting(id);

        [MonoPInvokeCallback(typeof(Action<string, string>))]
        private static void HandleOnUserDisconnecting(string id, string unused2) => instance.FireOnUserDisconnecting(id);

        /// <inheritdoc/>
        protected override string GetClientId() => WebGLHelper.CallFunction(WithPrefix(nameof(GetClientId)));

        protected override void DoReleaseManagedResources() => cancellation?.Dispose();

        protected override async UniTask<StartHostResponse> DoStartHostAsync(string name)
        {
            WebGLHelper.CallAction(WithPrefix(nameof(DoStartHostAsync)), name);
            await UniTask.WaitUntil(() => startHostResponse != null, cancellationToken: cancellation.Token);
            var result = startHostResponse;
            startHostResponse = null;
            return result;
        }

        protected override async UniTask<ListHostsResponse> DoListHostsAsync()
        {
            WebGLHelper.CallAction(WithPrefix(nameof(DoListHostsAsync)));
            await UniTask.WaitUntil(() => listHostsResponse != null, cancellationToken: cancellation.Token);
            var result = listHostsResponse;
            listHostsResponse = null;
            return result;
        }

#pragma warning disable CS1998
        protected override async UniTask DoStartClientAsync(string hostId)
            => WebGLHelper.CallAction(WithPrefix(nameof(DoStartClientAsync)), hostId);
#pragma warning restore CS1998

#pragma warning disable CS1998
        protected override async UniTask DoStopAsync()
        {
            cancellation.Cancel();
            cancellation.Dispose();
            cancellation = new CancellationTokenSource();
            WebGLHelper.CallAction(WithPrefix(nameof(DoStopAsync)));
        }
#pragma warning restore CS1998

        private static string WithPrefix(string name) => $"{nameof(WebGLPeerClient)}#{name}";

        private static string ToJson(WebGLPeerConfig peerConfig)
        {
            var jsonRtcConfiguration = new JsonRtcConfiguration
            {
                IceServers = peerConfig.IceServerConfigs.Count > 0
                    ? peerConfig.IceServerConfigs.Select(iceServerConfig => new JsonRtcIceServer
                    {
                        Urls = iceServerConfig.Urls.ToArray(),
                        Username = iceServerConfig.Username,
                        Credential = iceServerConfig.Credential
                    }).ToArray()
                    : Array.Empty<JsonRtcIceServer>()
            };
            var socketOptions = peerConfig.SocketOptions;
            var webGLSocketOptions = peerConfig.WebGLSocketOptions;
            var jsonSocketOptions = new JsonSocketOptions
            {
                ConnectionTimeout = (long)socketOptions.ConnectionTimeout.TotalMilliseconds,
                Reconnection = socketOptions.Reconnection,
                WithCredentials = webGLSocketOptions.WithCredentials,
            };
            var jsonPeerConfig = new JsonPeerConfig
            {
                Url = peerConfig.SignalingUrl,
                SocketOptions = jsonSocketOptions,
                PcConfig = jsonRtcConfiguration,
                VanillaIceTimeout = (long)peerConfig.VanillaIceTimeout.TotalMilliseconds,
                IsDebug = peerConfig.IsDebug
            };
            return JsonSerializer.Serialize(jsonPeerConfig);
        }
    }

    [SuppressMessage("Usage", "CC0047")]
    public class JsonPeerConfig
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("socketOptions")]
        public JsonSocketOptions SocketOptions { get; set; }

        [JsonPropertyName("pcConfig")]
        public JsonRtcConfiguration PcConfig { get; set; }

        [JsonPropertyName("vanillaIceTimeout")]
        public long VanillaIceTimeout { get; set; }

        [JsonPropertyName("isDebug")]
        public bool IsDebug { get; set; }
    }

    [SuppressMessage("Usage", "CC0047")]
    public class JsonSocketOptions
    {
        [JsonPropertyName("connectionTimeout")]
        public long ConnectionTimeout { get; set; }

        [JsonPropertyName("reconnection")]
        public bool Reconnection { get; set; }

        [JsonPropertyName("withCredentials")]
        public bool WithCredentials { get; set; }
    }

    [SuppressMessage("Usage", "CC0047")]
    public class JsonRtcConfiguration
    {
        [JsonPropertyName("iceServers")]
        public JsonRtcIceServer[] IceServers { get; set; }
    }

    [SuppressMessage("Usage", "CC0047")]
    public class JsonRtcIceServer
    {
        [JsonPropertyName("urls")]
        public string[] Urls { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("credential")]
        public string Credential { get; set; }
    }
}
#endif
