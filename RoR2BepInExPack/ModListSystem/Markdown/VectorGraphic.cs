using System;
using System.Runtime.CompilerServices;
using RoR2BepInExPack.ModListSystem.Markdown.Images;
using UnityEngine;

[assembly: InternalsVisibleTo("ModListSystem.Editor")]

namespace RoR2BepInExPack.ModListSystem.Markdown;

public class VectorGraphic : ScriptableObject
{
    private VectorImage _image;
    public string svgContents;

    public int QualityScaleFactor
    {
        get => _image.QualityScale;
        set => _image.QualityScale = value;
    }

    public Texture2D Texture { get; private set; }
    
    private void OnValidate()
    {
        CreatePreviewTexture();
    }
    
    private void OnEnable()
    {
        CreatePreviewTexture();
    }

    private void OnDisable()
    {
        if (_image != null)
        {
            _image.OnTextureResized -= OnTextureResized;
            _image.Dispose();
        }
        
        _image = null;
    }
    
    private void CreatePreviewTexture()
    {
        if (_image != null)
        {
            _image.OnTextureResized -= OnTextureResized;
            _image.Dispose();
        }
        
        if (string.IsNullOrEmpty(svgContents))
            return;
        
        _image = VectorImage.FromSvg(svgContents);
        _image.OnTextureResized += OnTextureResized;

        if (Texture)
            DestroyImmediate(Texture, true);
        
        Texture = FlipTextureVertically(_image.Texture);
    }

    private void OnTextureResized()
    {
        if (Texture)
            DestroyImmediate(Texture, true);
        
        Texture = FlipTextureVertically(_image.Texture);
    }
    
    private static Texture2D FlipTextureVertically(Texture2D tex)
    {
        var texData = tex.GetRawTextureData();
        var bytesPerRow = texData.Length / tex.width;
        var pixels = new byte[texData.Length];

        for (int top = 0; top < tex.height; top++)
        {
            var bot = tex.height - (top + 1);

            if (top == bot)
                continue;

            var topIndex = top * bytesPerRow;
            var botIndex = bot * bytesPerRow;
                
            // Copy from top to bottom
            Array.Copy(texData, topIndex, pixels, botIndex, bytesPerRow);
        }
            
        var flippedTex = new Texture2D(tex.width, tex.height, tex.format, false);
            
        flippedTex.LoadRawTextureData(pixels);
        flippedTex.Apply(false);

        return flippedTex;
    }
}
