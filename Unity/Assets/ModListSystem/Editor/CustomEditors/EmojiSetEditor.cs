using RoR2BepInExPack.ModListSystem.Markdown;
using UnityEditor;

namespace ModListSystem.Editor.CustomEditors
{
    [CustomEditor(typeof(EmojiSet))]
    public class EmojiSetEditor : UnityEditor.Editor
    {
        private EmojiSet Target => (EmojiSet)target;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            // EditorGUILayout.
        }
    }
}
