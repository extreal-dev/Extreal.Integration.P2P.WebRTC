#if !UNITY_WEBGL || UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Extreal.Core.Logging;
using Unity.WebRTC;

namespace Extreal.Integration.P2P.WebRTC.MVS.ClientControl
{
    public class NativeFailureClient
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(NativeDataChannelClient));

        private static readonly string Label = "sample";

        private readonly Dictionary<string, RTCDataChannel> dcDict;

        public NativeFailureClient(NativePeerClient peerClient)
        {
            dcDict = new Dictionary<string, RTCDataChannel>();
            peerClient.AddPcCreateHook((id, isOffer, pc) => throw new System.Exception("PeerClient Create Error Test"));
            peerClient.AddPcCloseHook(id => throw new System.Exception("PeerClient Close Error Test"));
        }
    }
}
#endif
