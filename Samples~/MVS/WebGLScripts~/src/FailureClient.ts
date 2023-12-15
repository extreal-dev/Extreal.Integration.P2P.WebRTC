import { PeerClientProvider } from "@extreal-dev/extreal.integration.p2p.webrtc";

class FailureClient {
  private static createPc = (id: string, isOffer: boolean, pc: RTCPeerConnection) => {
    throw new Error("CreatePeerClient Error Test");
  };

  private static closePc = (id: string) => {
    throw new Error("ClosePeerClient Error Test");
  };

  static failureHook(getPeerClient: PeerClientProvider) {
    getPeerClient().addPcCreateHook(FailureClient.createPc)
    getPeerClient().addPcCloseHook(FailureClient.closePc);
  }
}

export { FailureClient };
