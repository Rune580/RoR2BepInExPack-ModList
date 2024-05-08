using System;
using SkiaSharp;
using Svg.Skia;
using UnityEngine;

namespace RoR2BepInExPack.ModListSystem.Markdown.Images;

internal class VectorImage : BaseImage
{
    private readonly SKSvg _svg;
    private readonly SKPicture _skPicture;
    private readonly SKColorSpace _skColorSpace;
    private readonly SKColorType _skColorType;
    private readonly Vector2 _size;
    private int _qualityScale;
    private Texture2D _texture;

    public Action OnTextureResized;

    public override Texture2D Texture => _texture;

    public override float Width => _size.x;

    public override float Height => _size.y;

    public int QualityScale
    {
        get => _qualityScale;
        set
        {
            var oldScale = _qualityScale;
            _qualityScale = value;

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (oldScale != _qualityScale)
                ResizeTexture();
        }
    }

    public VectorImage(string imagePath) : base(imagePath)
    {
        _svg = SKSvg.CreateFromFile(imagePath);
        _skPicture = _svg.Picture ?? throw new InvalidOperationException();
        _skColorSpace = SKColorSpace.CreateSrgb();
        _skColorType = SKImageInfo.PlatformColorType;

        var size = _skPicture.CullRect.Size;
        _size = new Vector2(size.Width, size.Height);
        _qualityScale = 1;
        
        CreateTexture();
        UpdateTexture();
    }

    private void CreateTexture()
    {
        var unityTexFmt = _skColorType == SKColorType.Rgba8888 ? TextureFormat.RGBA32 : TextureFormat.BGRA32;

        var width = Mathf.CeilToInt(_size.x * _qualityScale);
        var height = Mathf.CeilToInt(_size.y * _qualityScale);
        
        _texture = new Texture2D(width, height, unityTexFmt, false, true)
        {
            wrapMode = TextureWrapMode.Clamp,
            hideFlags = HideFlags.HideAndDontSave
        };
    }

    private void ResizeTexture()
    {
        if (_texture)
            UnityObject.DestroyImmediate(_texture);
        
        CreateTexture();
        UpdateTexture();
        OnTextureResized?.Invoke();
    }

    private void UpdateTexture()
    {
        using var bitmap = _skPicture.ToBitmap(SKColor.Empty, _qualityScale, _qualityScale, _skColorType, SKAlphaType.Premul, _skColorSpace);
        if (bitmap is null)
            throw new InvalidOperationException("Failed to convert SVG to bitmap!");

        using var pixels = bitmap.PeekPixels();
        
        Texture.LoadRawTextureData(pixels.GetPixels(), pixels.RowBytes * pixels.Height);
        Texture.Apply(false);
    }

    public override void Dispose()
    {
        _skColorSpace.Dispose();
        _svg.Dispose();
    }
}
