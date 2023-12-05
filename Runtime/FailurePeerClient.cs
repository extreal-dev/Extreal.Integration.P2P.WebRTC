#if !UNITY_WEBGL || UNITY_EDITOR
namespace Extreal.Integration.P2P.WebRTC
{
    public class FailurePeerClient
    {
        /// <summary>
        /// Creates NativeTextChatClient with peerClient.
        /// </summary>
        /// <param name="peerClient">Peer client.</param>
        public FailurePeerClient(NativePeerClient peerClient)
        {
            peerClient.AddPcCreateHook((id, isOffer, pc) => throw new System.Exception("PeerClient Error Test"));
            peerClient.AddPcCloseHook(id => throw new System.Exception("PeerClient Error Test"));
        }
    }
}
#endif
