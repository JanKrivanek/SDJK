using SCKRM;
using UnityEngine;
using UnityEngine.UI;

namespace SDJK.Ruleset.ResultScreen
{
    public sealed class ReplayResultUIRankProgress : ReplayResultUIBase
    {
        [SerializeField, NotNull] Image background;
        [SerializeField, NotNull] Image color;

        double scoreAnimation = 0;
        public override void RealUpdate(float lerpValue)
        {
            scoreAnimation = scoreAnimation.Lerp(replay.scores.GetValue(double.MaxValue), lerpValue);
            float fillAmout = (float)(scoreAnimation / JudgementManager.maxScore);

            background.fillAmount = 1 - fillAmout;
            color.fillAmount = fillAmout;
        }

        public override void Remove()
        {
            scoreAnimation = 0;

            background.fillAmount = 1;
            color.fillAmount = 0;
        }
    }
}