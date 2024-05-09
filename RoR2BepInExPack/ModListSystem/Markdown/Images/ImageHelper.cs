using System;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2BepInExPack.ModListSystem.Markdown.Images;

internal static class ImageHelper
{
    private static ImageCache _cacheInstance;

    private static ImageCache GetImageCache()
    {
        if (_cacheInstance is not null)
            return _cacheInstance;
        
        var cacheFilePath = ImageCache.GetCacheFilePath();
        if (!File.Exists(cacheFilePath))
        {
            _cacheInstance = new ImageCache();
            SaveCache();
            
            return _cacheInstance;
        }

        try
        {
            _cacheInstance = JsonConvert.DeserializeObject<ImageCache>(File.ReadAllText(cacheFilePath));
        }
        catch (Exception e)
        {
            Debug.LogError(e);

            _cacheInstance = new ImageCache();
        }
        
        _cacheInstance.DeleteExpiredEntries();
        
        return _cacheInstance;
    }

    private static void SaveCache()
    {
        if (_cacheInstance is null)
        {
            Debug.LogWarning("Can't save null Image Cache!");
            return;
        }

        if (!EnsureCacheDir())
        {
            Debug.LogWarning("Image Cache path is invalid!");
            return;
        }
        
        var cacheContents = JsonConvert.SerializeObject(_cacheInstance, Formatting.None);
        File.WriteAllText(ImageCache.GetCacheFilePath(), cacheContents);
    }

    /// <summary>
    /// Creates the required directories for the Image Cache.
    /// </summary>
    /// <returns>If the directories exist and are valid</returns>
    private static bool EnsureCacheDir()
    {
        var cacheFilePath = ImageCache.GetCacheFilePath();
        
        var parent = Directory.GetParent(cacheFilePath);
        if (parent is null)
            return false;
        
        if (!parent.Exists)
            Directory.CreateDirectory(parent.FullName);

        return true;
    }
    
    public static BaseImage GetImage(string url)
    {
        var cache = GetImageCache();
        
        if (cache.TryGetImageEntry(url, out var imageEntry))
            return CreateImage(imageEntry.FullPath, imageEntry.ImageType);
        
        var request = UnityWebRequest.Get(url);
        
        request.SendWebRequest();
        while (!request.isDone)
            Thread.Sleep(100);

        var imageType = DetermineImageTypeFromHeader(request.downloadHandler);

        var fileName = Guid.NewGuid().ToString();
        
        switch (imageType)
        {
            case ImageType.Unknown:
                Debug.LogWarning($"Unknown Image Type!\n Url: {url}");
                break;
            case ImageType.Svg:
                using (var fs = cache.AddAndOpenWrite(url, $"{fileName}.svg", imageType))
                {
                    var bytes = request.downloadHandler.data;
                    fs.Write(bytes, 0, bytes.Length);
                }
                // using (var svg = SKSvg.CreateFromSvg(text))
                // {
                //     using var fs = cache.AddAndOpenWrite(url, $"{fileName}.png", imageType);
                //     svg.Save(fs, SKColor.Empty);
                // }
                break;
            case ImageType.Png:
                using (var fs = cache.AddAndOpenWrite(url, $"{fileName}.png", imageType))
                {
                    var bytes = request.downloadHandler.data;
                    fs.Write(bytes, 0, bytes.Length);
                }
                break;
            case ImageType.Jpeg:
                using (var fs = cache.AddAndOpenWrite(url, $"{fileName}.jpeg", imageType))
                {
                    var bytes = request.downloadHandler.data;
                    fs.Write(bytes, 0, bytes.Length);
                }
                break;
            case ImageType.Gif:
                using (var fs = cache.AddAndOpenWrite(url, $"{fileName}.gif", imageType))
                {
                    var bytes = request.downloadHandler.data;
                    fs.Write(bytes, 0, bytes.Length);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        SaveCache();
        
        if (cache.TryGetImageEntry(url, out imageEntry))
            return CreateImage(imageEntry.FullPath, imageEntry.ImageType);
        
        return null;
    }

    private static BaseImage CreateImage(string imagePath, ImageType imageType)
    {
        if (imageType == ImageType.Unknown)
        {
            Debug.LogWarning($"Unknown Image Type!");
            throw new InvalidOperationException();
        }
        
        if (imageType == ImageType.Svg)
            return VectorImage.FromFile(imagePath);

        if (imageType == ImageType.Gif) // Todo animated pngs and webps
            return new AnimatedImage(imagePath);

        return new StaticImage(imagePath);
    }

    private static ImageType DetermineImageTypeFromHeader(DownloadHandler handler)
    {
        // Try and detect SVG first.
        if (handler.text.TrimStart().StartsWith("<svg"))
            return ImageType.Svg;

        var bytes = handler.data;

        if (bytes.Length < ImageHeaderInspector.MaxHeaderLength) // Verify that the data is longer than the longest header we support (jpeg)
            return ImageType.Unknown;

        var imageInspector = new ImageHeaderInspector(bytes);

        // PNG files always have these 8 bytes at the start
        // http://www.libpng.org/pub/png/spec/1.2/PNG-Structure.html
        if (imageInspector.MatchForward(0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A))
            return ImageType.Png;

        if (imageInspector.MatchAndMoveForward(0xFF, 0xD8, 0xFF))
        {
            var appByte = imageInspector.GetCurrent();

            if (appByte is >= 0xE0 and <= 0xEF)
                return ImageType.Jpeg;
        }
        
        imageInspector.Reset();
        if (imageInspector.MatchForward("GIF"u8))
            return ImageType.Gif;
        
        return ImageType.Unknown;
    }
}
