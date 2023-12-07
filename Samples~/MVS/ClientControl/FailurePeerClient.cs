namespace Extreal.Integration.P2P.WebRTC.MVS.ClientControl
    {
        /// <summary>
        /// Creates NativeTextChatClient with peerClient.
        /// </summary>
        /// <param name="peerClient">Peer client.</param>
        public FailurePeerClient(NativePeerClient peerClient)
        {
            peerClient.AddPcCreateHook((id, isOffer, pc) => throw new System.Exception("PeerClient Create Error Test"));
            peerClient.AddPcCloseHook(id => throw new System.Exception("PeerClient Close Error Test"));
        }
    }
}
