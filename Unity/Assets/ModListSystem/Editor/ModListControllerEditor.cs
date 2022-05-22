using BepInEx;
using RoR2BepInExPack.ModListSystem;
using RoR2BepInExPack.ModListSystem.Components.ModList;
using UnityEditor;
using UnityEngine;

namespace ModListSystem.Editor
{
    [UnityEditor.CustomEditor(typeof(ModListController))]
    public class ModListControllerEditor : UnityEditor.Editor
    {
        private int _testCount = 1000;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            _testCount = EditorGUILayout.IntField("Data Size", _testCount);

            if (GUILayout.Button("Test"))
            {
                var controller = (ModListController)target;
                
                controller.UpdateDataSet(GenTestData(_testCount));
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
