import { PeerClientProvider } from "@extreal-dev/extreal.integration.p2p.webrtc";

class DataChannelClient {
    private readonly label: string = "sample";
    private readonly isDebug: boolean;
    private readonly dcMap: Map<string, RTCDataChannel>;
    private readonly getPeerClient: PeerClientProvider;

    constructor(getPeerClient: PeerClientProvider) {
        this.isDebug = true;
        this.dcMap = new Map();
        this.getPeerClient = getPeerClient;
        this.getPeerClient().addPcCreateHook(this.createPc);
        this.getPeerClient().addPcCloseHook(this.closePc);
    }

    private createPc = (id: string, isOffer: boolean, pc: RTCPeerConnection) => {
        if (this.dcMap.has(id)) {
            return;
        }

        if (isOffer) {
            const dc = pc.createDataChannel(this.label);
            this.handleDc(id, dc);
        } else {
            pc.addEventListener("datachannel", (event) => this.handleDc(id, event.channel));
        }
    };

    private handleDc = (id: string, dc: RTCDataChannel) => {
        if (dc.label !== this.label) {
            return;
        }

        if (this.isDebug) {
            console.log(`New DataChannel: id=${id} label=${dc.label}`);
        }

        this.dcMap.set(id, dc);
    };

    private closePc = (id: string) => {
        const dc = this.dcMap.get(id);
        if (!dc) {
            return;
        }
        dc.close();
        this.dcMap.delete(id);
    };

    public clear = () => {
        [...this.dcMap.keys()].forEach(this.closePc);
        this.dcMap.clear();
    };
}

export { DataChannelClient };
