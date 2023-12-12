using System.Diagnostics.CodeAnalysis;
using Extreal.Core.Common.System;
using Extreal.Integration.P2P.WebRTC.MVS.App;
using UniRx;
using VContainer.Unity;

namespace Extreal.Integration.P2P.WebRTC.MVS.ClientControl
{
    public class ClientControlPresenter : DisposableBase, IInitializable
    {
        private readonly AppState appState;
        private readonly PeerClient peerClient;

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        [SuppressMessage("CodeCracker", "CC0057")]
        public ClientControlPresenter(AppState appState, PeerClient peerClient)
        {
            this.appState = appState;
            this.peerClient = peerClient;
        }

        public void Initialize()
        {
            peerClient.OnStarted
                .Subscribe(id => appState.Notify($"Received: {nameof(PeerClient.OnStarted)}\nid: {id}"))
                .AddTo(disposables);

            peerClient.OnStartFailed
                .Subscribe(_ => appState.Notify($"Received: {nameof(PeerClient.OnStartFailed)}"))
                .AddTo(disposables);

            peerClient.OnConnectFailed
                .Subscribe(_ => appState.Notify($"Received: {nameof(PeerClient.OnConnectFailed)}"))
                .AddTo(disposables);

            peerClient.OnDisconnected
                .Subscribe(_ => appState.Notify($"Received: {nameof(PeerClient.OnDisconnected)}"))
                .AddTo(disposables);

            peerClient.OnUserConnected
                .Subscribe(id => appState.Notify($"Received: {nameof(PeerClient.OnUserConnected)}\nid: {id}"))
                .AddTo(disposables);

            peerClient.OnUserDisconnected
                .Subscribe(id => appState.Notify($"Received: {nameof(PeerClient.OnUserDisconnected)}\nid: {id}"))
                .AddTo(disposables);
        }
    }
}
