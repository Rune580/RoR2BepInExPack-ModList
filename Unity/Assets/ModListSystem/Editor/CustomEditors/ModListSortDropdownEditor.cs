using RoR2BepInExPack.ModListSystem.Components.ModList;
using UnityEditor;

namespace ModListSystem.Editor.CustomEditors
{
    [CustomEditor(typeof(ModListSortDropdown))]
    public class ModListSortDropdownEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}
