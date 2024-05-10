using System.IO;
using RoR2BepInExPack.ModListSystem.Markdown;
using UnityEditor;
using UnityEngine;

namespace ModListSystem.Editor.CustomEditors
{
    [CustomEditor(typeof(EmojiSet))]
    public class EmojiSetEditor : UnityEditor.Editor
    {
        private EmojiSet Target => (EmojiSet)target;

        private string _svgImportDir = "";

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Select Svg Import Dir"))
                _svgImportDir = EditorUtility.OpenFolderPanel("Select Svg Import Dir", Application.dataPath, "");
                    
            _svgImportDir = EditorGUILayout.TextField("Svg Import Dir", _svgImportDir);

            if (GUILayout.Button("Import"))
                ImportSvgs();
        }

        private void ImportSvgs()
        {
            Debug.Log(_svgImportDir);

            var files = Directory.GetFiles(_svgImportDir, "*.svg", SearchOption.AllDirectories);
            
            Target.Clear();
            
            serializedObject.Update();

            foreach (var file in files)
            {
                var assetPath = $"Assets{file.Replace(Application.dataPath, "")}";
                
                var codePoint = Path.GetFileNameWithoutExtension(file);
                var vectorGraphic = AssetDatabase.LoadAssetAtPath<VectorGraphic>(assetPath);
                
                Target.AddEmoji(codePoint, vectorGraphic);
            }

            serializedObject.ApplyModifiedProperties();
            
            EditorUtility.SetDirty(Target);
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log("Done!");
        }
    }
}
