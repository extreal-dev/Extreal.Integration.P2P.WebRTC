/**
 * Peer role.
 */
const PeerRole = {
    /**
     * None.
     */
    None: 0,
    /**
     * Host.
     */
    Host: 1,
    /**
     * Client.
     */
    Client: 2,
} as const;

type PeerRole = typeof PeerRole[keyof typeof PeerRole];

export { PeerRole };
