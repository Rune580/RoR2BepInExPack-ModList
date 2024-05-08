using System.IO;
using UnityEngine;

namespace RoR2BepInExPack.ModListSystem.Markdown.Images;

internal class StaticImage : BaseImage
{
    public override Texture2D Texture { get; }

    public StaticImage(string imagePath) : base(imagePath)
    {
        var imageBytes = File.ReadAllBytes(imagePath);
        
        Texture = new Texture2D(1, 1)
        {
            hideFlags = HideFlags.HideAndDontSave
        };
        
        Texture.LoadImage(imageBytes, true);
    }

    public override void Dispose()
    {
        UnityObject.DestroyImmediate(Texture, true);
    }
}
