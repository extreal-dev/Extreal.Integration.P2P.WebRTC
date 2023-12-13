#if !UNITY_WEBGL || UNITY_EDITOR
using System.Collections.Generic;
using Unity.WebRTC;

namespace Extreal.Integration.P2P.WebRTC.MVS.ClientControl
{
    public class NativeFailureClient
    {
        public static void NativeFailureHook(NativePeerClient peerClient)
        {
            peerClient.AddPcCreateHook((id, isOffer, pc) => throw new System.Exception("PeerClient Create Error Test"));
            peerClient.AddPcCloseHook(id => throw new System.Exception("PeerClient Close Error Test"));
        }
    }
}
#endif
