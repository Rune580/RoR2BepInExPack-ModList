using RoR2BepInExPack.ModListSystem.Components.Markdown;
using UnityEditor;
using UnityEngine;

namespace ModListSystem.Editor.CustomEditors
{
    [CustomEditor(typeof(MarkdownConverter))]
    public class MarkdownConverterEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            var markdownConverter = (MarkdownConverter)target;
            
            if (GUILayout.Button("Convert"))
            {
                markdownConverter.Awake();
            }

            if (GUILayout.Button("Clear"))
            {
                markdownConverter.ClearContent();
            }
        }
    }
}
