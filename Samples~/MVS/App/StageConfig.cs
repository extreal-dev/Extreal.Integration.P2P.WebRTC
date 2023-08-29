using Extreal.Core.StageNavigation;
using UnityEngine;

namespace Extreal.Integration.P2P.WebRTC.MVS.App
{
    [CreateAssetMenu(
        menuName = "P2P.WebRTC/" + nameof(StageConfig),
        fileName = nameof(StageConfig))]
    public class StageConfig : StageConfigBase<StageName, SceneName>
    {
    }
}
