type OnStarted = () => void;
type OnStartedFailed = () => void;

class ClientState {
    private readonly onStarted: OnStarted;
    private readonly onStartedFailed: OnStartedFailed;
    private isIceCandidateGatheringFinished: boolean;
    private isOfferAnswerProcessFinished: boolean;
    private isHostConnected: boolean;

    constructor(onStarted: OnStarted, onStartedFailed: OnStartedFailed) {
        this.onStarted = onStarted;
        this.onStartedFailed = onStartedFailed;
        this.isIceCandidateGatheringFinished = false;
        this.isOfferAnswerProcessFinished = false;
        this.isHostConnected = false;
    }

    public finishIceCandidateGathering = () => {
        this.isIceCandidateGatheringFinished = true;
        this.fireOnStarted();
    };

    public finishOfferAnswerProcess = () => {
        this.isOfferAnswerProcessFinished = true;
        this.fireOnStarted();
    };

    public finishHostConnection = (isConnectionWithHost: boolean) => {
        if (isConnectionWithHost) {
            this.isHostConnected = true;
            this.fireOnStarted();
        } else {
            this.isHostConnected = false;
        }
    };

    private fireOnStarted = () => {
        if (this.isIceCandidateGatheringFinished && this.isOfferAnswerProcessFinished) {
            this.onStarted();
        }
    };

    public fireOnClientStarted = () => {
        if (!this.isHostConnected) { return }
        this.fireOnStarted ();
    }

    public fireOnStartedFailed = () => {
            this.onStartedFailed();
    };

    public clear = () => {
        this.isIceCandidateGatheringFinished = false;
        this.isOfferAnswerProcessFinished = false;
        this.isHostConnected = false;
    };
}

export type { OnStarted };
    export { ClientState, OnStartedFailed };
