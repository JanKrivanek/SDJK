using SCKRM.Object;
using UnityEngine;

namespace SDJK.Effect
{
    public sealed class VideoEffect : Effect
    {
        [SerializeField] string _prefab = "video_effect.video"; public string prefab { get => _prefab; set => _prefab = value; }
        public VideoEffectPrefab video { get; private set; } = null;

        string lastVideoBackgroundFile;
        string lastVideoBackgroundNightFile;
        double lastVideoOffset;
        public override void Refresh(bool force = false)
        {
            if (video != null && !video.isRemoved)
                video.PadeOut().Forget();

            if (force || (lastVideoBackgroundFile != map.info.videoBackgroundFile || lastVideoBackgroundNightFile != map.info.videoBackgroundNightFile || lastVideoOffset != map.info.videoOffset))
            {
                video = (VideoEffectPrefab)ObjectPoolingSystem.ObjectCreate(prefab, transform, false).monoBehaviour;
                video.Refresh(effectManager.selectedMap, effectManager.soundPlayer);

                lastVideoBackgroundFile = map.info.videoBackgroundFile;
                lastVideoBackgroundNightFile = map.info.videoBackgroundNightFile;
                lastVideoOffset = map.info.videoOffset;
            }
        }
    }
}
