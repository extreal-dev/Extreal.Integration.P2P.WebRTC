#if !UNITY_WEBGL || UNITY_EDITOR
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Extreal.Core.Common.System;
using UniRx;

namespace Extreal.Integration.P2P.WebRTC
{
    internal class NativeClientState : DisposableBase
    {
        [SuppressMessage("Usage", "CC0033")]
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        internal IObservable<Unit> OnStarted => onStarted.AddTo(disposables);
        [SuppressMessage("Usage", "CC0033")]
        private readonly Subject<Unit> onStarted = new Subject<Unit>();

        [SuppressMessage("Usage", "CC0033")]
        private readonly Subject<bool> isIceCandidateGatheringFinished = new Subject<bool>();
        [SuppressMessage("Usage", "CC0033")]
        private readonly Subject<bool> isOfferAnswerProcessFinished = new Subject<bool>();

        [SuppressMessage("CodeCracker", "CC0092")]
        internal NativeClientState()
        {
            isIceCandidateGatheringFinished.AddTo(disposables);
            isOfferAnswerProcessFinished.AddTo(disposables);
            Observable.CombineLatest(isIceCandidateGatheringFinished, isOfferAnswerProcessFinished)
                .Where(readies => readies.All(ready => ready))
                .Subscribe(_ => onStarted.OnNext(Unit.Default))
                .AddTo(disposables);
        }

        internal void FinishIceCandidateGathering() => isIceCandidateGatheringFinished.OnNext(true);
        internal void FinishOfferAnswerProcess() => isOfferAnswerProcessFinished.OnNext(true);

        internal void Clear()
        {
            isIceCandidateGatheringFinished.OnNext(false);
            isOfferAnswerProcessFinished.OnNext(false);
        }

        protected override void ReleaseManagedResources() => disposables.Dispose();
    }
}
#endif
