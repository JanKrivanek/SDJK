using SCKRM.Rhythm;
using SDJK.Effect;
using SDJK.Map.Ruleset.SDJK.Map;
using UnityEngine;

namespace SDJK.Ruleset.SDJK.Effect
{
    public class PlayFieldEffect : SDJKEffect
    {
        public override EffectManager effectManager => playField.effectManager;

        [SerializeField] PlayField playField;
        SDJKFieldEffectFile fieldEffectFile => playField.fieldEffectFile;

        protected override void RealUpdate()
        {
            PosUpdate();
            SizeUpdate();
        }

        void PosUpdate()
        {
            transform.localPosition = fieldEffectFile.pos.GetValue(RhythmManager.currentBeatScreen);
            transform.localEulerAngles = fieldEffectFile.rotation.GetValue(RhythmManager.currentBeatScreen);
        }

        void SizeUpdate()
        {
            Camera camera = Camera.main;
            float screenY;
            if (camera.orthographic)
                screenY = camera.orthographicSize * 2;
            else
                screenY = Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad) * 2.0f * CameraEffect.defaultDistance;

            float scale = (float)(screenY / playField.fieldHeight);
            Vector3 effectScale = (Vector3)fieldEffectFile.scale.GetValue(RhythmManager.currentBeatScreen);

            transform.localScale = new Vector3(scale * effectScale.x, scale * effectScale.y, scale * effectScale.z);
        }
    }
}
