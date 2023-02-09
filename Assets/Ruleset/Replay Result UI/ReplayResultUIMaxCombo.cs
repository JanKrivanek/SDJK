using SCKRM;
using TMPro;
using UnityEngine;

namespace SDJK.Ruleset.ResultScreen
{
    public sealed class ReplayResultUIMaxCombo : ReplayResultUIBase
    {
        [SerializeField, NotNull] TMP_Text text;

        double maxComboAnimation = 0;
        public override void RealUpdate(float lerpValue)
        {
            maxComboAnimation = maxComboAnimation.Lerp(replay.maxCombo.GetValue(double.MaxValue), lerpValue);
            text.text = maxComboAnimation.RoundToInt().ToString();
        }

        public override void Remove()
        {
            maxComboAnimation = 0;
            text.text = "0";
        }
    }
}