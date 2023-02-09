using SCKRM;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SDJK.Ruleset.ReplayResult
{
    public sealed class ReplayResultUIRank : ReplayResultUIBase
    {
        [SerializeField, NotNull] Graphic background;
        [SerializeField, NotNull] TMP_Text text;

        double accuracyAnimation = 1;
        public override void RealUpdate(float lerpValue)
        {
            accuracyAnimation = accuracyAnimation.Lerp(replay.accuracys.GetValue(double.MaxValue), lerpValue);

            RankMetaData rank = ruleset.GetRank(accuracyAnimation);
            background.color = rank.color;
            text.text = rank.name;
        }

        public override void ObjectReset()
        {
            base.ObjectReset();
            accuracyAnimation = 1;

            background.color = Color.clear;
            text.text = "";
        }
    }
}
