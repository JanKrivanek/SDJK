using SCKRM;
using SDJK.Map;
using SDJK.Replay;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SDJK.Ruleset.UI.ReplayResult
{
    public sealed class ReplayResultUIDifficultyText : ReplayResultUIBase
    {
        [SerializeField, FieldNotNull] ColorBand gradient;

        [SerializeField, FieldNotNull] Image background;
        [SerializeField, FieldNotNull] TMP_Text text;

        public override void Refresh(IRuleset ruleset, MapFile map, ReplayFile replay)
        {
            base.Refresh(ruleset, map, replay);
            double difficulty = replay.mapDifficultyAverage;

            background.color = gradient.Evaluate((float)(difficulty / 10d));
            text.text = difficulty.ToString("0.00");
        }

        public override void ObjectReset()
        {
            base.ObjectReset();
            text.text = "";
        }
    }
}
