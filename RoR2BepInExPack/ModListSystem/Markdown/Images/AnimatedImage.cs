using System;
using System.IO;
using SkiaSharp;
using UnityEngine;

namespace RoR2BepInExPack.ModListSystem.Markdown.Images;

internal class AnimatedImage : BaseImage
{
    private readonly MemoryStream _stream;
    private readonly SKManagedStream _skStream;
    private readonly SKCodec _codec;
    private readonly SKCodecFrameInfo[] _frames;
    private readonly SKImageInfo _imageInfo;
    private readonly Memory<byte> _memoryBuffer;

    private int _priorFrame = -1;
    private int _currentFrame;

    public int FrameCount => _frames.Length;
    
    public float FrameDurationMs => _frames[_currentFrame].Duration / 1000f;
    
    public override Texture2D Texture { get; }
    
    public AnimatedImage(string imagePath)
    {
        _stream = new MemoryStream(File.ReadAllBytes(imagePath));
        _skStream = new SKManagedStream(_stream);

        _codec = SKCodec.Create(_skStream);
        _frames = _codec.FrameInfo;

        var info = _codec.Info;
        _imageInfo = new SKImageInfo(info.Width, info.Height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);

        var unityTexFmt = _imageInfo.ColorType == SKColorType.Rgba8888 ? TextureFormat.RGBA32 : TextureFormat.BGRA32;
        Texture = new Texture2D(_imageInfo.Width, _imageInfo.Height, unityTexFmt, false, true)
        {
            wrapMode = TextureWrapMode.Clamp,
            hideFlags = HideFlags.HideAndDontSave
        };
        
        _memoryBuffer = new Memory<byte>(new byte[_imageInfo.RowBytes * _imageInfo.Height]);
        
        UpdateTexture();
    }

    public void SetFrame(int frame)
    {
        if (frame < 0 || frame >= FrameCount)
            return;

        _priorFrame = -1;
        _currentFrame = frame;
    }

    public void NextFrame()
    {
        _priorFrame = _currentFrame;
        _currentFrame++;

        if (_currentFrame >= FrameCount)
        {
            _currentFrame = 0;
            _priorFrame = -1;
        }
        
        UpdateTexture();
    }

    private void UpdateTexture()
    {
        unsafe
        {
            var opts = _priorFrame >= 0
                ? new SKCodecOptions(_currentFrame, _priorFrame)
                : new SKCodecOptions(_currentFrame);

            using var handle = _memoryBuffer.Pin();
            var ptr = (IntPtr)handle.Pointer;

            if (_codec.GetPixels(_imageInfo, ptr, opts) != SKCodecResult.Success)
            {
                Debug.LogError("Failed to get frame in Gif!");
                return;
            }
            
            Texture.LoadRawTextureData(ptr, _memoryBuffer.Length);
            Texture.Apply(false);
        }
    }

    public override void Dispose()
    {
        _codec.Dispose();
        _skStream.Dispose();
        _stream.Dispose();
        
        UnityObject.DestroyImmediate(Texture, true);
    }
}
