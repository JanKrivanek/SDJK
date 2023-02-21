using SCKRM;
using SCKRM.Rhythm;
using SDJK.Ruleset.SuperHexagon.Judgement;
using UnityEngine;
using UnityEngine.UI;

namespace SDJK.Ruleset.SuperHexagon.UI
{
    public sealed class TimeSliderUI : SuperHexagonUIBase
    {
        [SerializeField, NotNull] Image fill;

        void Update()
        {
            if (!RhythmManager.isPlaying || SuperHexagonJudgementManager.instance == null)
                return;

            fill.fillAmount = (float)(RhythmManager.currentBeat / judgementManager.map.info.clearBeat).Clamp01();
        }
    }
}
