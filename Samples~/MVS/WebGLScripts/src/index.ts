import { addAction } from "@extreal-dev/extreal.integration.web.common";
import { PeerAdapter } from "@extreal-dev/extreal.integration.p2p.webrtc";
import { DataChannelClient } from "./DataChannelClient";

const peerAdapter = new PeerAdapter();
peerAdapter.adapt();
const dataChannelClient = new DataChannelClient(peerAdapter.getPeerClient);

addAction("clear", dataChannelClient.clear);
