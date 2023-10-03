type OnStarted = () => void;
type OnStartedFailed = () => void;

class ClientState {
    private readonly onStarted: OnStarted;
    private readonly onStartedFailed: OnStartedFailed;
    private isIceCandidateGatheringFinished: boolean;
    private isOfferAnswerProcessFinished: boolean;

    constructor(onStarted: OnStarted, onStartedFailed: OnStartedFailed) {
        this.onStarted = onStarted;
        this.onStartedFailed = onStartedFailed;
        this.isIceCandidateGatheringFinished = false;
        this.isOfferAnswerProcessFinished = false;
    }

    public finishIceCandidateGathering = () => {
        this.isIceCandidateGatheringFinished = true;
        this.fireOnStarted();
    };

    public finishOfferAnswerProcess = () => {
        this.isOfferAnswerProcessFinished = true;
        this.fireOnStarted();
    };

    private fireOnStarted = () => {
        if (this.isIceCandidateGatheringFinished && this.isOfferAnswerProcessFinished) {
            this.onStarted();
        }
    };

    public fireOnStartedFailed = () => {
            this.onStartedFailed();
    };

    public clear = () => {
        this.isIceCandidateGatheringFinished = false;
        this.isOfferAnswerProcessFinished = false;
    };
}

export type { OnStarted };
export { ClientState, OnStartedFailed };
