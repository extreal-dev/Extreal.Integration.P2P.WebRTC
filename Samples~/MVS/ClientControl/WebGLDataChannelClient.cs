using Extreal.Integration.Web.Common;

namespace Extreal.Integration.P2P.WebRTC.MVS.ClientControl
{
    public class WebGLDataChannelClient : DataChannelClient
    {
        public WebGLDataChannelClient() => WebGLHelper.CallAction("start");

        public override void Clear() => WebGLHelper.CallAction("clear");
    }
}
