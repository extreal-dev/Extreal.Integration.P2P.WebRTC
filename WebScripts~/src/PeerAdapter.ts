import { PeerClient } from "./PeerClient";
import { addAction, callback } from "@extreal-dev/extreal.integration.web.common";

type PeerClientProvider = () => PeerClient;

/**
 * Class that defines the PeerClient integration between C# and JavaScript.
 */
class PeerAdapter {
    private peerClient: PeerClient | undefined;

    /**
     * Adapts the PeerClient integration between C# and JavaScript.
     */
    public adapt = () => {
        addAction(this.withPrefix("WebGLPeerClient"), (jsonPeerConfig) => {
            const peerConfig = JSON.parse(jsonPeerConfig);
            if (peerConfig.isDebug) {
                console.log(peerConfig);
            }
            this.peerClient = new PeerClient(peerConfig, {
                onStarted: () => callback(this.withPrefix("HandleOnStarted")),
                onStartFailed: () => callback(this.withPrefix("HandleOnStartFailed")),
                onSignalingConnectFailed: (reason) => callback(this.withPrefix("HandleOnConnectFailed"), reason),
                onSignalingDisconnected: (reason) => callback(this.withPrefix("HandleOnSignalingDisconnected"), reason),
            });
        });

        addAction(this.withPrefix("DoStartHostAsync"), (name) => {
            this.getPeerClient().startHost(name, (response) =>
                callback(this.withPrefix("ReceiveStartHostResponse"), JSON.stringify(response)),
            );
        });

        addAction(this.withPrefix("DoListHostsAsync"), () => {
            this.getPeerClient().listHosts((response) =>
                callback(this.withPrefix("ReceiveListHostsResponse"), JSON.stringify(response)),
            );
        });

        addAction(
            this.withPrefix("DoStartClientAsync"),
            async (hostId) => await this.getPeerClient().startClientAsync(hostId),
        );

        addAction(this.withPrefix("DoStopAsync"), () => this.getPeerClient().stop());
    };

    private withPrefix = (name: string) => `WebGLPeerClient#${name}`;

    /**
     * Gets the PeerClient.
     * @return Function to get PeerClient. 
     */
    public getPeerClient: PeerClientProvider = () => {
        if (!this.peerClient) {
            throw new Error("Call the WebGLPeerClient constructor first in Unity.");
        }
        return this.peerClient;
    };
}

export { PeerAdapter, PeerClientProvider };
