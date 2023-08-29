import { addAction } from "@extreal-dev/extreal.integration.web.common";
import { PeerAdapter } from "@extreal-dev/extreal.integration.p2p.webrtc";
import { DataChannelClient } from "./DataChannelClient";

const peerAdapter = new PeerAdapter();
peerAdapter.adapt();

let dataChannelClient: DataChannelClient;
addAction("start", () => dataChannelClient = new DataChannelClient(peerAdapter.getPeerClient));
addAction("clear", () => dataChannelClient.clear);
