using RoR2BepInExPack.ModListSystem.Components.ModList;
using UnityEditor;

namespace ModListSystem.Editor
{
    [CustomEditor(typeof(ModListSortChoice))]
    public class ModListSortChoiceEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}
