type OnStarted = () => void;
type OnStartFailed = () => void;

class ClientState {
    private readonly onStarted: OnStarted;
    private readonly onStartFailed: OnStartFailed;
    private isIceCandidateGatheringFinished: boolean;
    private isOfferAnswerProcessFinished: boolean;

    constructor(onStarted: OnStarted, onStartFailed: OnStartFailed) {
        this.onStarted = onStarted;
        this.onStartFailed = onStartFailed;
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

    public fireOnStartFailed = () => {
            this.onStartFailed();
    };

    public clear = () => {
        this.isIceCandidateGatheringFinished = false;
        this.isOfferAnswerProcessFinished = false;
    };
}

export type { OnStarted };
export { ClientState, OnStartFailed };
