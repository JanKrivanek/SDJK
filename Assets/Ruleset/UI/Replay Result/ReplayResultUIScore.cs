using SCKRM;
using TMPro;
using UnityEngine;

namespace SDJK.Ruleset.UI.ReplayResult
{
    public sealed class ReplayResultUIScore : ReplayResultUIBase
    {
        [SerializeField, FieldNotNull] TMP_Text text;

        double scoreAnimation = 0;
        public override void RealUpdate(float lerpValue)
        {
            scoreAnimation = scoreAnimation.Lerp(replay.scores.GetValue(double.MaxValue), lerpValue);
            text.text = scoreAnimation.Round().ToString();
        }

        public override void ObjectReset()
        {
            base.ObjectReset();

            scoreAnimation = 0;
            text.text = "";
        }
    }
}
