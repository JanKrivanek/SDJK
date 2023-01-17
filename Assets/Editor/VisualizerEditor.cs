using SCKRM.Editor;
using SDJK.Effect;
using UnityEditor;
using UnityEngine;

namespace SDJK.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Visualizer))]
    public class VisualizerEditor : CustomInspectorEditor
    {
        Visualizer editor;

        protected override void OnEnable()
        {
            base.OnEnable();
            editor = (Visualizer)target;
        }

        public override void OnInspectorGUI()
        {
            UseProperty("barPrefab", "바 프리팹");

            Space();

            UseProperty("_circle");

            Space();

            editor.all = EditorGUILayout.Toggle("한번에 변경", editor.all);

            Space();

            if (!editor.all)
            {
                editor.left = EditorGUILayout.Toggle("왼쪽으로 애니메이션", editor.left);
                editor.divide = EditorGUILayout.IntField("분할", editor.divide);

                Space();
            }

            editor.offset = EditorGUILayout.IntField("오프셋", editor.offset);
            editor.size = EditorGUILayout.FloatField("크기", editor.size);

            Space();

            UseProperty("_length");

            if (GUI.changed)
                EditorUtility.SetDirty(target);
        }
    }
}
