using RoR2BepInExPack.ModListSystem.Markdown;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ModListSystem.Editor.CustomEditors
{
    [CustomEditor(typeof(VectorGraphic))]
    public class VectorGraphicEditor : UnityEditor.Editor
    {
        private VectorGraphic Target => (VectorGraphic)target;

        public override bool HasPreviewGUI() => Target.Texture;

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            GUI.DrawTexture(r, Target.Texture, ScaleMode.ScaleToFit);
        }

        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            if (!Target || !Target.Texture)
                return null;

            Target.QualityScaleFactor = 4;

            var staticPreview = new Texture2D(width, height);
            EditorUtility.CopySerialized(Target.Texture, staticPreview);

            return staticPreview;
        }
    }
}
