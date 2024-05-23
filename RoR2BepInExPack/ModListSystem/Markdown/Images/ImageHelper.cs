using System;
using System.Collections;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
// using UnityEngine.Networking;

namespace RoR2BepInExPack.ModListSystem.Markdown.Images;

internal static class ImageHelper
{
    private static readonly HttpClient Client = new();
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
    
    public static IEnumerator GetImage(string url, Action<BaseImage> getImageCallback)
    {
        var cache = GetImageCache();

        if (cache.TryGetImageEntry(url, out var imageEntry))
        {
            getImageCallback.Invoke(CreateImage(imageEntry.FullPath, imageEntry.ImageType));
            yield break;
        }

        var getImageBytesTask = Task.Run(async () => await GetImageBytesHttp(url));
        yield return new WaitUntil(() => getImageBytesTask.IsCompleted);

        var imageBytes = getImageBytesTask.Result;
        if (imageBytes is null)
        {
            getImageCallback.Invoke(null);
            yield break;
        }
        
        var imageType = DetermineImageTypeFromHeader(imageBytes);

        var fileName = Guid.NewGuid().ToString();
        
        switch (imageType)
        {
            case ImageType.Unknown:
                Debug.LogWarning($"Unknown Image Type!\n Url: {url}");
                break;
            case ImageType.Svg:
                using (var fs = cache.AddAndOpenWrite(url, $"{fileName}.svg", imageType))
                {
                    fs.Write(imageBytes, 0, imageBytes.Length);
                }
                break;
            case ImageType.Png:
                using (var fs = cache.AddAndOpenWrite(url, $"{fileName}.png", imageType))
                {
                    fs.Write(imageBytes, 0, imageBytes.Length);
                }
                break;
            case ImageType.Jpeg:
                using (var fs = cache.AddAndOpenWrite(url, $"{fileName}.jpeg", imageType))
                {
                    fs.Write(imageBytes, 0, imageBytes.Length);
                }
                break;
            case ImageType.Gif:
                using (var fs = cache.AddAndOpenWrite(url, $"{fileName}.gif", imageType))
                {
                    fs.Write(imageBytes, 0, imageBytes.Length);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        SaveCache();

        if (cache.TryGetImageEntry(url, out imageEntry))
        {
            getImageCallback.Invoke(CreateImage(imageEntry.FullPath, imageEntry.ImageType));
            yield break;
        }
        
        getImageCallback.Invoke(null);
    }

    private static async Task<byte[]> GetImageBytesHttp(string url)
    {
        using var response = await Client.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            return null;
        
        return await response.Content.ReadAsByteArrayAsync();
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

    private static ImageType DetermineImageTypeFromHeader(byte[] buffer)
    {
        var textBuf = new byte[Mathf.Min(buffer.Length, 4)];
        Array.Copy(buffer, textBuf, textBuf.Length);

        var textContent = Encoding.UTF8.GetString(textBuf);
        
        // Try and detect SVG first.
        if (textContent.TrimStart().StartsWith("<svg"))
            return ImageType.Svg;

        if (buffer.Length < ImageHeaderInspector.MaxHeaderLength) // Verify that the data is longer than the longest header we support (jpeg)
            return ImageType.Unknown;

        var imageInspector = new ImageHeaderInspector(buffer);

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
