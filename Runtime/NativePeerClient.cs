#if !UNITY_WEBGL || UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using Cysharp.Threading.Tasks;
using Extreal.Core.Logging;
using SocketIOClient;
using UniRx;
using Unity.WebRTC;

namespace Extreal.Integration.P2P.WebRTC
{
    public class NativePeerClient : PeerClient
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(NativePeerClient));

        private readonly PeerConfig peerConfig;
        private RTCConfiguration pcConfig;
        private SocketIO socket;
        private readonly Dictionary<string, RTCPeerConnection> pcDict;
        private readonly List<Action<string, bool, RTCPeerConnection>> pcCreateHooks;
        private readonly List<Action<string>> pcCloseHooks;
        private readonly NativeClientState clientState;
        private CancellationTokenSource cancellation;

        /// <summary>
        /// Role of P2P communication
        /// </summary>
        public PeerRole Role { get; private set; }
        /// <summary>
        /// HostId of P2P communication
        /// </summary>
        public string HostId { get; private set; }

        public NativePeerClient(PeerConfig peerConfig) : base(peerConfig)
        {
            this.peerConfig = peerConfig;
            pcConfig = ToPcConfig(peerConfig);
            pcDict = new Dictionary<string, RTCPeerConnection>();
            pcCreateHooks = new List<Action<string, bool, RTCPeerConnection>>();
            pcCloseHooks = new List<Action<string>>();
            clientState = new NativeClientState();
            Disposables.Add(clientState);
            cancellation = new CancellationTokenSource();

            clientState.OnStarted.Subscribe(_ => FireOnStarted()).AddTo(Disposables);

            Role = PeerRole.None;

            LogIceServers(pcConfig);
        }

        private static void LogIceServers(RTCConfiguration pcConfig)
        {
            if (Logger.IsDebug())
            {
                if (pcConfig.iceServers is null)
                {
                    // Not covered by testing due to defensive implementation
                    Logger.LogDebug("Ice server: None");
                }
                else
                {
                    foreach (var iceServer in pcConfig.iceServers)
                    {
                        Logger.LogDebug($"Ice server: urls={string.Join(", ", iceServer.urls)} username={iceServer.username} credential={iceServer.credential}");
                    }
                }
            }
        }

        private static RTCConfiguration ToPcConfig(PeerConfig peerConfig)
            => peerConfig.IceServerConfigs.Count > 0
                ? new RTCConfiguration
                {
                    iceServers = peerConfig.IceServerConfigs.Select(iceServerConfig => new RTCIceServer
                    {
                        urls = iceServerConfig.Urls.ToArray(),
                        username = iceServerConfig.Username,
                        credential = iceServerConfig.Credential
                    }).ToArray()
                }
                : new RTCConfiguration();

        protected override void DoReleaseManagedResources()
        {
            cancellation?.Dispose();
            socket?.Dispose();
        }

        /// <summary>
        /// Add a process to be called when creating a peer connection
        /// </summary>
        /// <param name="hook"></param>
        public void AddPcCreateHook(Action<string, bool, RTCPeerConnection> hook)
            => pcCreateHooks.Add(hook);

        /// <summary>
        /// Add a process to be called when a peer connection is terminated
        /// </summary>
        /// <param name="hook"></param>
        public void AddPcCloseHook(Action<string> hook)
            => pcCloseHooks.Add(hook);

        private async UniTask<SocketIO> GetSocketAsync()
        {
            if (socket is not null)
            {
                if (socket.Connected)
                {
                    return socket;
                }
                // Not covered by testing due to defensive implementation
                await StopSocketAsync();
            }

            socket = new SocketIO(peerConfig.SignalingUrl, peerConfig.SocketOptions);
            socket.OnConnected += ReceiveConnected;
            socket.On("message", ReceiveMessageAsync);
            socket.On("user disconnected", ReceiveUserDisconnectedAsync);
            socket.OnDisconnected += ReceiveDisconnectedAsync;

            try
            {
                await socket.ConnectAsync().ConfigureAwait(true);
            }
            catch (ConnectionException e)
            {
                FireOnConnectFailed(e.Message);
                throw;
            }

            return socket;
        }

        private void ReceiveConnected(object sender, EventArgs e)
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"Socket created: id={socket.Id}");
            }
        }

        private async void ReceiveMessageAsync(SocketIOResponse response)
        {
            await UniTask.SwitchToMainThread();

            if (Logger.IsDebug())
            {
                Logger.LogDebug($"Receive message: {response}");
            }

            var message = response.GetValue<Message>();
            switch (message.Type)
            {
                case "join":
                {
                    await ReceiveJoinAsync(message.From);
                    break;
                }
                case "call me":
                {
                    await SendOfferAsync(message.Me);
                    break;
                }
                case "offer":
                {
                    await ReceiveOfferAsync(message.From, message.ToSd());
                    break;
                }
                case "answer":
                {
                    await ReceiveAnswerAsync(message.From, message.ToSd());
                    break;
                }
                case "done":
                {
                    ReceiveDone(message.From);
                    break;
                }
                case "bye":
                {
                    ReceiveBye(message.From);
                    break;
                }
                default:
                {
                    // Not covered by testing due to defensive implementation
                    if (Logger.IsDebug())
                    {
                        Logger.LogDebug($"Unknown message received!!! type={message.Type}");
                    }
                    break;
                }
            }
        }

        private async void ReceiveUserDisconnectedAsync(SocketIOResponse response)
        {
            await UniTask.SwitchToMainThread();

            if (Logger.IsDebug())
            {
                Logger.LogDebug($"Receive user disconnected: {response}");
            }

            var userDisconnected = response.GetValue<UserDisconnected>();
            ClosePc(userDisconnected.Id);
        }

        private async void ReceiveDisconnectedAsync(object sender, string reason)
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"{nameof(ReceiveDisconnectedAsync)}: {reason}");
            }
            await UniTask.SwitchToMainThread();
            FireOnDisconnected(reason);
        }

        protected override async UniTask<StartHostResponse> DoStartHostAsync(string name)
        {
            Role = PeerRole.Host;

            StartHostResponse startHostResponse = null;
            await (await GetSocketAsync()).EmitAsync("create host", response =>
            {
                if (Logger.IsDebug())
                {
                    Logger.LogDebug(response.ToString());
                }
                startHostResponse = response.GetValue<StartHostResponse>();
            }, name).ConfigureAwait(true);

            await UniTask.WaitUntil(() => startHostResponse != null, cancellationToken: cancellation.Token);
            return startHostResponse;
        }

        protected override async UniTask<ListHostsResponse> DoListHostsAsync()
        {
            ListHostsResponse listHostsResponse = null;
            await (await GetSocketAsync()).EmitAsync("list hosts", response =>
            {
                if (Logger.IsDebug())
                {
                    Logger.LogDebug(response.ToString());
                }
                listHostsResponse = response.GetValue<ListHostsResponse>();
            }).ConfigureAwait(true);

            await UniTask.WaitUntil(() => listHostsResponse != null, cancellationToken: cancellation.Token);
            return listHostsResponse;
        }

        protected override async UniTask DoStartClientAsync(string hostId)
        {
            Role = PeerRole.Client;
            HostId = hostId;
            await SendMessageAsync(HostId, new Message { Type = "join" });
        }

        private async UniTask SendOfferAsync(string to)
        {
            if (pcDict.ContainsKey(to))
            {
                // Not covered by testing due to defensive implementation
                if (Logger.IsDebug())
                {
                    Logger.LogDebug($"Send offer: Not sent as it already exists. to={to}");
                }
                return;
            }

            CreatePc(to, true);

            await HandlePcAsync(
                nameof(SendOfferAsync),
                to,
                async (pc) =>
                {
                    var offerAsyncOp = pc.CreateOffer();
                    await offerAsyncOp;
                    var sd = offerAsyncOp.Desc;
                    await pc.SetLocalDescription(ref sd);
                    SendSdpByCompleteOrTimeoutAsync(to, pc).Forget();
                });
        }

        protected override async UniTask DoStopAsync()
        {
            cancellation.Cancel();
            cancellation.Dispose();
            cancellation = new CancellationTokenSource();

            Role = PeerRole.None;
            HostId = null;

            foreach (var id in pcDict.Keys.ToList())
            {
                await SendMessageAsync(id, new Message { Type = "bye" });
                ClosePc(id);
            }

            pcDict.Clear();
            clientState.Clear();
            await StopSocketAsync();
        }

        private async UniTask StopSocketAsync()
        {
            if (socket is null)
            {
                // Not covered by testing due to defensive implementation
                return;
            }
            socket.OnConnected -= ReceiveConnected;
            socket.OnDisconnected -= ReceiveDisconnectedAsync;
            await socket.DisconnectAsync().ConfigureAwait(true);
            socket.Dispose();
            socket = null;
        }

        private void CreatePc(string id, bool isOffer)
        {
            if (pcDict.ContainsKey(id))
            {
                // Not covered by testing due to defensive implementation
                return;
            }

            var pc = new RTCPeerConnection(ref pcConfig);

            pc.OnIceConnectionChange += _ =>
            {
                if (Logger.IsDebug())
                {
                    Logger.LogDebug($"Receive ice connection change: state={pc.IceConnectionState} id={id}");
                }
                switch (pc.IceConnectionState)
                {
                    case RTCIceConnectionState.New:
                    case RTCIceConnectionState.Checking:
                    case RTCIceConnectionState.Disconnected:
                    case RTCIceConnectionState.Max:
                    {
                        // do nothing
                        break;
                    }
                    case RTCIceConnectionState.Connected:
                    case RTCIceConnectionState.Completed:
                    {
                        if (Role == PeerRole.Client && id == HostId)
                        {
                            clientState.FinishIceCandidateGathering();
                        }
                        break;
                    }
                    case RTCIceConnectionState.Failed:
                    case RTCIceConnectionState.Closed:
                    {
                        // Not covered by testing due to defensive implementation
                        ClosePc(id);
                        break;
                    }
                    default:
                    {
                        // Not covered by testing due to defensive implementation
                        throw new Exception("Unexpected Case");
                    }
                }
            };

            FireOnUserConnecting(id);

            pcCreateHooks.ForEach(hook => HandleHook(nameof(CreatePc), () => hook.Invoke(id, isOffer, pc)));
            pcDict.Add(id, pc);
        }

        private static void HandleHook(string name, Action hook)
        {
            try
            {
                hook.Invoke();
            }
            catch (Exception e)
            {
                Logger.LogError($"Error has occured at {name}", e);
            }
        }

        private async UniTask SendSdpByCompleteOrTimeoutAsync(string to, RTCPeerConnection pc)
        {
            var isTimeout = false;
            try
            {
                await UniTask
                    .WaitUntil(() => pc.GatheringState == RTCIceGatheringState.Complete)
                    .Timeout(peerConfig.VanillaIceTimeout);
            }
            catch (TimeoutException)
            {
                isTimeout = true;
            }
            if (Logger.IsDebug())
            {
                var result = isTimeout ? "timeout" : "complete";
                Logger.LogDebug($"Vanilla ICE gathering: {result} id={to}");
            }
            SendSdpAsync(to, pc.LocalDescription).Forget();
        }

        private void ClosePc(string from)
            => HandlePc(
                nameof(ClosePc),
                from,
                pc =>
                {
                    FireOnUserDisconnecting(from);
                    pcCloseHooks.ForEach(hook => HandleHook(nameof(ClosePc), () => hook.Invoke(from)));
                    pc.Close();
                    pcDict.Remove(from);
                });

        private UniTask SendSdpAsync(string to, RTCSessionDescription sd)
            => SendMessageAsync(to, new Message
            {
                Type = sd.type.ToString().ToLower(),
                Sdp = sd.sdp,
            });

        [SuppressMessage("Design", "CC0021")]
        private async UniTask SendMessageAsync(string to, Message message)
        {
            message.To = to;
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"Send message: {message}");
            }
            await (await GetSocketAsync()).EmitAsync("message", message).ConfigureAwait(true);
        }

        private async UniTask ReceiveJoinAsync(string from)
        {
            await SendOfferAsync(from);
            foreach (var to in pcDict.Keys)
            {
                if (from == to)
                {
                    continue;
                }
                SendMessageAsync(to, new Message { Type = "call me", Me = from }).Forget();
            }
        }

        private async UniTask ReceiveOfferAsync(string from, RTCSessionDescription sd)
        {
            CreatePc(from, false);

            await HandlePcAsync(
                nameof(ReceiveOfferAsync),
                from,
                async pc =>
                {
                    await pc.SetRemoteDescription(ref sd);
                    await SendAnswerAsync(from);
                });
        }

        private UniTask SendAnswerAsync(string from)
            => HandlePcAsync(
                nameof(SendAnswerAsync),
                from,
                async pc =>
                {
                    var answerAsyncOp = pc.CreateAnswer();
                    await answerAsyncOp;
                    var sd = answerAsyncOp.Desc;
                    await pc.SetLocalDescription(ref sd);
                    SendSdpByCompleteOrTimeoutAsync(from, pc).Forget();
                });

        private UniTask ReceiveAnswerAsync(string from, RTCSessionDescription sd)
            => HandlePcAsync(
                nameof(ReceiveAnswerAsync),
                from,
                async pc =>
                {
                    await pc.SetRemoteDescription(ref sd);
                    await SendMessageAsync(from, new Message { Type = "done" });
                });

        private void ReceiveDone(string from)
        {
            if (Role == PeerRole.Client && from == HostId)
            {
                clientState.FinishOfferAnswerProcess();
            }
        }

        private void ReceiveBye(string from) => ClosePc(from);

        private void HandlePc(
            string methodName,
            string id,
            Action<RTCPeerConnection> handle)
        {
            if (!pcDict.TryGetValue(id, out var pc))
            {
                // Not covered by testing due to defensive implementation
                return;
            }
            try
            {
                handle.Invoke(pc);
            }
            catch (Exception e)
            {
                // Not covered by testing due to defensive implementation
                if (Logger.IsDebug())
                {
                    Logger.LogDebug($"Error has occurred at {methodName}", e);
                }
            }
        }

        private async UniTask HandlePcAsync(
            string methodName,
            string id,
            Func<RTCPeerConnection, UniTask> handle)
        {
            if (!pcDict.TryGetValue(id, out var pc))
            {
                // Not covered by testing due to defensive implementation
                return;
            }
            try
            {
                await handle.Invoke(pc);
            }
            catch (Exception e)
            {
                // Not covered by testing due to defensive implementation
                if (Logger.IsDebug())
                {
                    Logger.LogDebug($"Error has occurred at {methodName}", e);
                }
            }
        }

        /// <inheritdoc/>
        protected override string GetClientId() => socket?.Id;
    }

    [SuppressMessage("Usage", "CC0047")]
    public class Message
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

#pragma warning disable CS8632

        [JsonPropertyName("from")]
        public string? From { get; set; }

        [JsonPropertyName("to")]
        public string? To { get; set; }

        [JsonPropertyName("me")]
        public string? Me { get; set; }

        [JsonPropertyName("sdp")]
        public string? Sdp { get; set; }

#pragma warning restore CS8632

        private static readonly Dictionary<string, RTCSdpType> TypeMapping
            = new Dictionary<string, RTCSdpType>
            {
                {"offer", RTCSdpType.Offer},
                {"answer", RTCSdpType.Answer},
            };

        public RTCSessionDescription ToSd()
            => new RTCSessionDescription
            {
                type = TypeMapping[Type],
                sdp = Sdp,
            };

        public override string ToString()
            => $"{nameof(Type)}: {Type}, {nameof(From)}: {From}, {nameof(To)}: {To}, "
               + $"{nameof(Me)}: {Me}, {nameof(Sdp)}: {Sdp}";
    }

    [SuppressMessage("Usage", "CC0047")]
    public class UserDisconnected
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}
#endif
