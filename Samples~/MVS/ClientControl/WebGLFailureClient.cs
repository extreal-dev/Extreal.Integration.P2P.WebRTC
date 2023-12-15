#if UNITY_WEBGL && !UNITY_EDITOR
using Extreal.Integration.Web.Common;

namespace Extreal.Integration.P2P.WebRTC.MVS.ClientControl
{
    public class WebGLFailureClient
    {
        public static void FailureConnect() => WebGLHelper.CallAction("failure");
    }
}
#endif
