using BepInEx;
using RoR2BepInExPack.ModListSystem;
using RoR2BepInExPack.ModListSystem.Components.ModList;
using UnityEngine;

namespace ModListSystem.Editor
{
    [UnityEditor.CustomEditor(typeof(ModListController))]
    public class ModListControllerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Test"))
            {
                var controller = (ModListController)target;
                
                controller.UpdateDataSet(GenTestData(30));
            }
        }

        private ModDataInfo[] GenTestData(int count)
        {
            ModDataInfo[] testData = new ModDataInfo[count];

            for (int i = 0; i < count; i++)
            {
                ModData modData = new ModDataBuilder()
                    .WithBepInPlugin($"com.test.mod_{i}", $"Mod {i + 1}", "1.0.0")
                    .InternalBuild();

                testData[i] = new ModDataInfo(new PluginInfo(), modData);
            }

            return testData;
        }
    }
}
