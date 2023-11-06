using Essentials;
using UnityEditor;
using UnityEngine;

namespace Essentials.Editor
{
    [CustomEditor(typeof(RenderSettingsData))]
    public class RenderSettingsDataEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var target = (RenderSettingsData) this.target;
            if (GUILayout.Button("Copy from scene"))
            {
                target.CopyFromScene();
            }
            else if (GUILayout.Button("Apply to scene"))
            {
                target.Apply();
            }
        }
    }
}