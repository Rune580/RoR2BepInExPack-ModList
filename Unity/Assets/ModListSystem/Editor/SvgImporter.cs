using System.IO;
using RoR2BepInExPack.ModListSystem.Markdown;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

namespace ModListSystem.Editor
{
    [ScriptedImporter(1, "svg")]
    public class SvgImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var vectorGraphic = ScriptableObject.CreateInstance<VectorGraphic>();
            vectorGraphic.svgContents = File.ReadAllText(ctx.assetPath);
            
            ctx.AddObjectToAsset("Vector Graphic", vectorGraphic);
            ctx.SetMainObject(vectorGraphic);
        }
    }
}
