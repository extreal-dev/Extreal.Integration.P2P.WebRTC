using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Cysharp.Threading.Tasks;
using Extreal.Core.Common.System;
using Extreal.Core.Logging;
using UniRx;

namespace Extreal.Integration.P2P.WebRTC
{
    /// <summary>
    /// Client class for P2P connections.
    /// </summary>
    public abstract class PeerClient : DisposableBase
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(PeerClient));

        /// <summary>
        /// Invokes immediately after the host or client starts.
        /// </summary>
        public IObservable<string> OnStarted => onStarted.AddTo(Disposables);
        [SuppressMessage("Usage", "CC0033")]
        private readonly Subject<string> onStarted = new Subject<string>();

        /// <summary>
        /// Invokes immediately after the host or client has failed to start.
        /// </summary>
        public IObservable<Unit> OnStartFailed => onStartFailed.AddTo(Disposables);
        [SuppressMessage("Usage", "CC0033")]
        private readonly Subject<Unit> onStartFailed = new Subject<Unit>();

        /// <summary>
        /// Invokes immediately after the host or client has failed to connect to the signaling server.
        /// </summary>
        public IObservable<string> OnConnectFailed => onConnectFailed.AddTo(Disposables);
        [SuppressMessage("Usage", "CC0033")]
        private readonly Subject<string> onConnectFailed = new Subject<string>();

        /// <summary>
        /// Invokes immediately after a host or client connected to the signaling server is disconnected.
        /// </summary>
        public IObservable<string> OnDisconnected => onDisconnected.AddTo(Disposables);
        [SuppressMessage("Usage", "CC0033")]
        private readonly Subject<string> onDisconnected = new Subject<string>();

        /// <summary>
        /// Invokes immediately before remote user connects.
        /// </summary>
        public IObservable<string> OnUserConnecting => onUserConnecting;
        [SuppressMessage("Usage", "CC0033")]
        private readonly Subject<string> onUserConnecting = new Subject<string>();

        /// <summary>
        /// Invokes immediately before remote user disconnects.
        /// </summary>
        public IObservable<string> OnUserDisconnecting => onUserDisconnecting;
        [SuppressMessage("Usage", "CC0033")]
        private readonly Subject<string> onUserDisconnecting = new Subject<string>();

        /// <summary>
        /// Whether it is running or not.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Disposables.
        /// </summary>
        protected CompositeDisposable Disposables { get; } = new CompositeDisposable();

        private readonly PeerConfig peerConfig;

        /// <summary>
        /// Get local client id.
        /// </summary>
        /// <returns>Local client id</returns>
        protected abstract string GetLocalClientId();

        /// <summary>
        /// Creates a new peer client.
        /// </summary>
        /// <param name="peerConfig">peer configuration</param>
        protected PeerClient(PeerConfig peerConfig)
        {
            this.peerConfig = peerConfig;
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"signaling url={peerConfig.SignalingUrl} "
                                + $"socket connection timeout={peerConfig.SocketOptions.ConnectionTimeout} "
                                + $"P2P timeout={peerConfig.P2PTimeout} "
                                + $"Vanilla ICE timeout={peerConfig.VanillaIceTimeout}");
            }
        }

        /// <summary>
        /// Fires the OnStarted.
        /// </summary>
        protected void FireOnStarted()
        {
            if (IsRunning)
            {
                // Not covered by testing due to defensive implementation
                return;
            }
            if (Logger.IsDebug())
            {
                Logger.LogDebug("P2P started");
            }
            IsRunning = true;
            onStarted.OnNext(GetLocalClientId());
        }

        /// <summary>
        /// Fires the OnConnectFailed.
        /// </summary>
        /// <param name="reason">Reason</param>
        protected void FireOnConnectFailed(string reason)
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"{nameof(FireOnConnectFailed)}: reason={reason}");
            }
            onConnectFailed.OnNext(reason);
        }

        /// <summary>
        /// Fires the OnDisconnected.
        /// </summary>
        /// <param name="reason">Reason</param>
        protected void FireOnDisconnected(string reason)
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"{nameof(FireOnDisconnected)}: reason={reason}");
            }
            if (reason == "io client disconnect")
            {
                // Not covered by testing due to defensive implementation
                return;
            }
            onDisconnected.OnNext(reason);
        }

        /// <summary>
        /// Fires the OnUserConnecting.
        /// </summary>
        /// <param name="id">Id</param>
        protected void FireOnUserConnecting(string id)
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"{nameof(FireOnUserConnecting)}: id={id}");
            }
            onUserConnecting.OnNext(id);
        }

        /// <summary>
        /// Fires the OnUserDisconnecting.
        /// </summary>
        /// <param name="id">Id</param>
        protected void FireOnUserDisconnecting(string id)
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"{nameof(FireOnUserDisconnecting)}: id={id}");
            }
            onUserDisconnecting.OnNext(id);
        }

        /// <inheritdoc/>
        protected sealed override void ReleaseManagedResources()
        {
            DoReleaseManagedResources();
            Disposables.Dispose();
        }

        /// <summary>
        /// Releases managed resources.
        /// </summary>
        protected abstract void DoReleaseManagedResources();

        /// <summary>
        /// Starts as host.
        /// </summary>
        /// <param name="name">Host name</param>
        /// <exception cref="HostNameAlreadyExistsException">When hostname already exists at creation of host.</exception>
        public async UniTask StartHostAsync(string name)
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"Start host: name={name}");
            }
            MonitorToStartAsync().Forget();

            var startHostResponse = await DoStartHostAsync(name);
            if (startHostResponse.Status == 409)
            {
                if (Logger.IsDebug())
                {
                    Logger.LogDebug(startHostResponse.Message);
                }
                throw new HostNameAlreadyExistsException(startHostResponse.Message);
            }
            else
            {
                FireOnStarted();
            }
        }

        /// <summary>
        /// Starts as host.
        /// </summary>
        /// <param name="name">Host name</param>
        /// <returns>Response</returns>
        protected abstract UniTask<StartHostResponse> DoStartHostAsync(string name);

        /// <summary>
        /// Lists hosts.
        /// </summary>
        /// <returns>Host list</returns>
        public async UniTask<List<Host>> ListHostsAsync()
        {
            var listHostsResponse = await DoListHostsAsync();
            return listHostsResponse.Hosts.Select(host => new Host(host.Id, host.Name)).ToList();
        }

        /// <summary>
        /// Lists hosts.
        /// </summary>
        /// <returns>Response</returns>
        protected abstract UniTask<ListHostsResponse> DoListHostsAsync();

        /// <summary>
        /// Starts as client.
        /// </summary>
        /// <param name="hostId">Host id to join</param>
        /// <returns>UniTask</returns>
        public UniTask StartClientAsync(string hostId)
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"Start client: hostId={hostId}");
            }
            MonitorToStartAsync().Forget();
            return DoStartClientAsync(hostId);
        }

        /// <summary>
        /// Starts as client.
        /// </summary>
        /// <param name="hostId">Host id to join</param>
        /// <returns>UniTask</returns>
        protected abstract UniTask DoStartClientAsync(string hostId);

        private async UniTask MonitorToStartAsync()
        {
            try
            {
                await UniTask.WaitUntil(() => IsRunning).Timeout(peerConfig.P2PTimeout);
            }
            catch (TimeoutException)
            {
                if (Logger.IsDebug())
                {
                    Logger.LogDebug("P2P start processing timed out");
                }
                Stop();
                onStartFailed.OnNext(Unit.Default);
            }
        }

        /// <summary>
        /// Stops the connection.
        /// </summary>
        public void Stop()
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug(nameof(Stop));
            }
            IsRunning = false;
            DoStopAsync().Forget();
        }

        /// <summary>
        /// Stops the connection.
        /// </summary>
        /// <returns>UniTask</returns>
        protected abstract UniTask DoStopAsync();
    }
}
