using SCKRM.SaveLoad.UI;
using UnityEditor;
using UnityEngine;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SaveLoadUIInputField))]
    public class SaveLoadUIInputFieldEditor : SaveLoadUIBaseEditor
    {
        [System.NonSerialized] SaveLoadUIInputField editor;

        protected override void OnEnable()
        {
            base.OnEnable();
            editor = (SaveLoadUIInputField)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawLine();

            UseProperty("_mouseSensitivity", "마우스 감도");

            DrawLine();

            editor.textPlaceHolderNameSpace = DrawNameSpace("플레이스홀더 네임스페이스", editor.textPlaceHolderNameSpace);
            UseProperty("_textPlaceHolderPath", "플레이스홀더 텍스트");

            DrawLine();

            editor.numberPlaceHolderNameSpace = DrawNameSpace("플레이스홀더 네임스페이스 (숫자)", editor.numberPlaceHolderNameSpace);
            UseProperty("_numberPlaceHolderPath", "플레이스홀더 텍스트 (숫자)");

            DrawLine();

            UseProperty("_inputField");
            UseProperty("_placeholder");

            DrawLine();

            UseProperty("_onEndEdit");

            if (GUI.changed)
                EditorUtility.SetDirty(target);
        }
    }
}