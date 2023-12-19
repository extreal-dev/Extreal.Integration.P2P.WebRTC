import { PeerClientProvider } from "@extreal-dev/extreal.integration.p2p.webrtc";

class DummyClient {
  private static createPcError = (id: string, isOffer: boolean, pc: RTCPeerConnection) => {
    throw new Error("CreatePeerClient Error Test");
  };

  private static closePcError = (id: string) => {
    throw new Error("ClosePeerClient Error Test");
  };

  private static createPcErrorAsync = async (id: string, isOffer: boolean, pc: RTCPeerConnection) => {
    throw new Error("CreatePeerClient Async Error Test");
  };

  private static createPcAsync = async (id: string, isOffer: boolean, pc: RTCPeerConnection) => {
    console.log("Dummy createPcAsync Start");
    await new Promise<void>(resolve => setTimeout(() => {
      console.log("Dummy createPcAsync Finish");
      resolve();
    }, 1000));
  };

  static dummyHook(getPeerClient: PeerClientProvider) {
    getPeerClient().addPcCreateHook(DummyClient.createPcError)
    getPeerClient().addPcCloseHook(DummyClient.closePcError);
    getPeerClient().addPcCreateHook(DummyClient.createPcErrorAsync)
    getPeerClient().addPcCreateHook(DummyClient.createPcAsync)
  }
}

export { DummyClient };
