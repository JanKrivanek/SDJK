using SCKRM.Rhythm;
using SCKRM.UI;
using UnityEngine;

namespace SDJK.Ruleset.SDJK
{
    public sealed class AccuracyBarRod : UIObjectPoolingBase
    {
        [SerializeField] float speed = 0.001f;

        Color defaultColor;
        public override void OnCreate()
        {
            base.OnCreate();

            rectTransform.anchoredPosition = Vector2.zero;
            defaultColor = graphic.color;
        }

        void Update()
        {
            Color color = graphic.color;
            color.a -= speed * RhythmManager.bpmFpsDeltaTime;

            graphic.color = color;

            if (color.a <= 0)
                Remove();
        }

        public override void Remove()
        {
            base.Remove();

            rectTransform.anchorMin = new Vector2(0.5f, rectTransform.anchorMin.y);
            rectTransform.anchorMax = new Vector2(0.5f, rectTransform.anchorMax.y);

            graphic.color = defaultColor;
        }
    }
}
