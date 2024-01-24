using System;
using System.Diagnostics.CodeAnalysis;
using Extreal.Core.Common.System;
using UniRx;

namespace Extreal.Integration.P2P.WebRTC.MVS.App
{
    public class AppState : DisposableBase
    {
        public IObservable<string> OnNotificationReceived => onNotificationReceived.AddTo(disposables);
        [SuppressMessage("Usage", "CC0033")]
        private readonly Subject<string> onNotificationReceived = new Subject<string>();

        public IObservable<string> OnSocketIdSet => onSocketIdSet.AddTo(disposables);
        [SuppressMessage("Usage", "CC0033")]
        private readonly Subject<string> onSocketIdSet = new Subject<string>();
        public void SetSocketId(string socketId) => onSocketIdSet.OnNext(socketId);

        [SuppressMessage("Usage", "CC0033")]
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        private PeerRole role = PeerRole.Host;
        private string groupName;
        private string groupId;

        public bool IsHost => role == PeerRole.Host;
        public bool IsClient => role == PeerRole.Client;
        public void SetRole(PeerRole role) => this.role = role;

        public string GroupName { get; private set; }
        public void SetGroupName(string groupName) => GroupName = groupName;

        public string GroupId { get; private set; }
        public void SetGroupId(string groupId) => GroupId = groupId;

        public void Notify(string message) => onNotificationReceived.OnNext(message);

        protected override void ReleaseManagedResources() => disposables.Dispose();
    }
}
