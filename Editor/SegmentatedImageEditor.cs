using UnityEditor;
using UnityEditor.UI;

namespace Essentials
{
    [CustomEditor(typeof(SegmentatedImage))]
    public class SegmentatedImageEditor : ImageEditor
    {
        private SerializedProperty segments = null;

        protected override void OnEnable()
        {
            base.OnEnable();
            segments = serializedObject.FindProperty("_segments");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.PropertyField(segments);

            serializedObject.ApplyModifiedProperties();
        }
    }
}