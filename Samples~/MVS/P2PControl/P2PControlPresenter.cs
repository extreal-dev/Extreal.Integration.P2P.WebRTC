using System;
using System.Diagnostics.CodeAnalysis;
using Cysharp.Threading.Tasks;
using Extreal.Core.Common.System;
using Extreal.Core.StageNavigation;
using Extreal.Integration.P2P.WebRTC.MVS.App;
using Extreal.Integration.P2P.WebRTC.MVS.ClientControl;
using UniRx;
using VContainer.Unity;

namespace Extreal.Integration.P2P.WebRTC.MVS.P2PControl
{
    public class P2PControlPresenter : DisposableBase, IInitializable
    {
        private readonly StageNavigator<StageName, SceneName> stageNavigator;
        private readonly AppState appState;
        private readonly PeerClient peerClient;
        private readonly DataChannelClient dataChannelClient;

        [SuppressMessage("Usage", "CC0033")]
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        private Action<string> handleHostNameAlreadyExistsException;

        public P2PControlPresenter(
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            PeerClient peerClient,
            DataChannelClient dataChannelClient)
        {
            this.stageNavigator = stageNavigator;
            this.appState = appState;
            this.peerClient = peerClient;
            this.dataChannelClient = dataChannelClient;
        }

        public void Initialize()
        {
            handleHostNameAlreadyExistsException = message =>
            {
                appState.Notify($"Received: {nameof(HostNameAlreadyExistsException)} {message}");
                stageNavigator.ReplaceAsync(StageName.GroupSelectionStage).Forget();
            };

            stageNavigator.OnStageTransitioned
                .Subscribe(_ => StartPeerClientAsync(appState).Forget())
                .AddTo(disposables);

            stageNavigator.OnStageTransitioning
                .Subscribe(_ =>
                {
                    dataChannelClient.Clear();
                    peerClient.Stop();
                })
                .AddTo(disposables);
        }

        private async UniTask StartPeerClientAsync(AppState appState)
        {
            try
            {
                if (appState.IsHost)
                {
                    await peerClient.StartHostAsync(appState.GroupName);
                }
                else
                {
                    await peerClient.StartClientAsync(appState.GroupId);
                }
            }
            catch (HostNameAlreadyExistsException e)
            {
                handleHostNameAlreadyExistsException.Invoke(e.Message);
            }
        }

        protected override void ReleaseManagedResources() => disposables.Dispose();
    }
}
