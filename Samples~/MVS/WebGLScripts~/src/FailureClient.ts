import { PeerClientProvider } from "@extreal-dev/extreal.integration.p2p.webrtc";

class FailureClient {
  private readonly getPeerClient: PeerClientProvider;

  constructor(getPeerClient: PeerClientProvider) {
    this.getPeerClient = getPeerClient;
    this.getPeerClient().addPcCreateHook(this.createPc);
    this.getPeerClient().addPcCloseHook(this.closePc);
  }

  private createPc = (id: string, isOffer: boolean, pc: RTCPeerConnection) => {
    throw new Error("CreatePeerClient Error Test");
  };

  private closePc = (id: string) => {
    throw new Error("ClosePeerClient Error Test");
  };
}

export { FailureClient };
