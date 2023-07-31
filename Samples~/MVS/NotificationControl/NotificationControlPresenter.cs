using Extreal.Core.Common.System;
using Extreal.Core.StageNavigation;
using Extreal.Integration.P2P.WebRTC.MVS.App;
using UniRx;
using VContainer.Unity;

namespace Extreal.Integration.P2P.WebRTC.MVS.NotificationControl
{
    public class NotificationControlPresenter : DisposableBase, IInitializable
    {
        private readonly AppState appState;
        private readonly NotificationControlView notificationControlView;

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        public NotificationControlPresenter(
            AppState appState,
            NotificationControlView notificationControlView)
        {
            this.appState = appState;
            this.notificationControlView = notificationControlView;
        }

        public void Initialize()
        {
            appState.OnNotificationReceived
                .Subscribe(notificationControlView.Show)
                .AddTo(disposables);

            notificationControlView.OnBackButtonClicked
                .Subscribe(_ => notificationControlView.Hide())
                .AddTo(disposables);
        }

        protected override void ReleaseManagedResources() => disposables.Dispose();
    }
}
