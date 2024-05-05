using System;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using Svg;
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
    
    public static Texture2D GetImage(string url)
    {
        var cache = GetImageCache();

        var tex2d = new Texture2D(1, 1);

        if (cache.TryGetImagePath(url, out var imagePath))
        {
            tex2d.LoadImage(File.ReadAllBytes(imagePath));
            return tex2d;
        }
        
        var request = UnityWebRequest.Get(url);
        
        request.SendWebRequest();
        while (!request.isDone)
            Thread.Sleep(100);

        var imageType = DetermineImageType(request.downloadHandler);

        switch (imageType)
        {
            case ImageType.Unknown:
                break;
            case ImageType.Svg:
                var fileName = $"{Guid.NewGuid().ToString()}.png";
                var text = request.downloadHandler.text;

                var svg = SvgDocument.FromSvg<SvgDocument>(text);
                var bitmap = svg.Draw();

                using (var fs = cache.AddAndOpenWrite(url, fileName))
                    bitmap.Save(fs, ImageFormat.Png);
                break;
            case ImageType.Png:
                break;
            case ImageType.Jpeg:
                break;
            case ImageType.Gif:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        SaveCache();
        
        if (cache.TryGetImagePath(url, out imagePath))
        {
            tex2d.LoadImage(File.ReadAllBytes(imagePath));
            return tex2d;
        }
        
        // Todo

        return null;
    }

    private static ImageType DetermineImageType(DownloadHandler handler)
    {
        if (handler.text.TrimStart().StartsWith("<svg"))
            return ImageType.Svg;

        // Todo detect other types of images
        return ImageType.Unknown;
    }
    
    private enum ImageType
    {
        Unknown,
        Svg,
        Png,
        Jpeg,
        Gif
    }
}
