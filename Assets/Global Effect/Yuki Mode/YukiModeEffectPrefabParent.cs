using SCKRM;
using SCKRM.Object;
using SCKRM.Rhythm;
using SDJK.Effect;

namespace SDJK
{
    public abstract class YukiModeEffectPrefabParent : ObjectPooling
    {
        public YukiModeEffect yukiModeEffect { get; private set; }

        public int indexOffset { get; private set; }
        public bool isLeft { get; private set; }

        public bool yukiMode { get; private set; } = false;

        public double offsetCurrentBeat { get; private set; } = 0;
        public double offsetCurrentBeatReapeat { get; private set; } = 0;

        double lastCurrentBeatReapeat = 0;
        protected virtual void Update()
        {
            if (yukiModeEffect == null)
                return;

            offsetCurrentBeat = RhythmManager.currentBeatScreen - indexOffset;
            offsetCurrentBeatReapeat = offsetCurrentBeat.Reapeat(yukiModeEffect.count.Ceil());

            if (offsetCurrentBeatReapeat < lastCurrentBeatReapeat)
                yukiMode = RhythmManager.screenYukiMode || yukiModeEffect.forceShow;

            lastCurrentBeatReapeat = offsetCurrentBeatReapeat;
        }

        public void Refresh(YukiModeEffect yukiModeEffect, int indexOffset, bool isLeft)
        {
            this.yukiModeEffect = yukiModeEffect;

            this.indexOffset = indexOffset;
            this.isLeft = isLeft;
        }

        public override bool Remove()
        {
            if (!base.Remove())
                return false;

            yukiModeEffect = null;

            indexOffset = 0;
            isLeft = false;

            yukiMode = false;

            offsetCurrentBeat = 0;
            offsetCurrentBeatReapeat = 0;

            lastCurrentBeatReapeat = 0;
            return true;
        }
    }
}
