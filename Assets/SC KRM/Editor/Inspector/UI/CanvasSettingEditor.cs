using SCKRM.UI;
using UnityEditor;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CanvasSetting))]
    public class CanvasSettingEditor : UIEditor
    {
        [System.NonSerialized] CanvasSetting editor;

        protected override void OnEnable()
        {
            base.OnEnable();
            editor = (CanvasSetting)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            UseProperty("_customSetting", "커스텀 설정");

            if (!editor.customSetting)
            {
                UseProperty("_worldRenderMode", "월드 렌더 모드");

                if (editor.worldRenderMode)
                    UseProperty("_planeDistance");

                Space();

                UseProperty("_forceSafeScreenEnable", "강제로 안전 스크린 활성화");
                UseProperty("_safeScreenMultiple", "안전 스크린 배수");
                UseProperty("_safeScreenOffset", "안전 스크린 오프셋");
            }

            if (!editor.customSetting || editor.canvas.renderMode != UnityEngine.RenderMode.WorldSpace)
            {
                DrawLine();

                UseProperty("_customGuiSize", "GUI 크기 커스텀");
                if (!editor.customGuiSize)
                    UseProperty("_guiSize", "GUI 크기");
            }

            DrawLine();

            UseProperty("_alwaysVisible", "항상 씬에 보이게");
        }
    }
}