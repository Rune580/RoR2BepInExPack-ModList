using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace RoR2BepInExPack.ModListSystem.Markdown.Images;

internal class ImageCache
{
    private static readonly string[] ValidExtensions = [ ".png", ".tiff", ".bmp", ".jpeg", ".jpg" ];

    [JsonProperty]
    private Dictionary<string, CacheEntry> UrlCacheEntryLut { get; set; } = new();

    public bool TryGetImagePath(string url, out string imagePath)
    {
        imagePath = null;
        
        if (!UrlCacheEntryLut.TryGetValue(url, out var entry))
            return false;

        if (entry.Expired || string.IsNullOrEmpty(entry.FileName))
        {
            RemoveEntry(url, entry);
            return false;
        }

        var imageExt = Path.GetExtension(entry.FileName);
        if (ValidExtensions.All(ext => ext != imageExt))
        {
            RemoveEntry(url, entry);
            return false;
        }

        imagePath = Path.GetFullPath(Path.Combine(GetCacheDir(), entry.FileName));
        if (!File.Exists(imagePath))
        {
            RemoveEntry(url, entry);
            return false;
        }
        
        return true;
    }

    public void DeleteExpiredEntries()
    {
        var expiredImages = UrlCacheEntryLut.Where(kvp => kvp.Value.Expired)
            .Select(kvp => kvp.Value.FileName);
        
        foreach (var expiredImageName in expiredImages)
            DeleteImage(expiredImageName);
        
        UrlCacheEntryLut = UrlCacheEntryLut.Where(kvp => !kvp.Value.Expired)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    private void RemoveEntry(string url, CacheEntry entry)
    {
        UrlCacheEntryLut.Remove(url);
        DeleteImage(entry.FileName);
    }

    private static void DeleteImage(string fileName)
    {
        var expiredExt = Path.GetExtension(fileName); 
        if (ValidExtensions.All(ext => expiredExt != ext))
            return;

        var fullPath = Path.GetFullPath(Path.Combine(GetCacheDir(), fileName));
        if (!File.Exists(fullPath))
            return;
            
        try
        {
            File.Delete(fullPath);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public FileStream AddAndOpenWrite(string url, string fileName)
    {
        var entry = new CacheEntry
        {
            FileName = fileName, ExpiresAfter = DateTimeOffset.Now.AddDays(1).ToUnixTimeMilliseconds()
        };

        if (UrlCacheEntryLut.TryGetValue(url, out var oldEntry))
            RemoveEntry(url, oldEntry);

        UrlCacheEntryLut[url] = entry;
        
        var filePath = Path.GetFullPath(Path.Combine(GetCacheDir(), fileName));
        return File.OpenWrite(filePath);
    }
    
    public static string GetCacheDir() => Path.Combine(Application.persistentDataPath, "RoR2BepInExPack", "Cache");
    public static string GetCacheFilePath() => Path.Combine(GetCacheDir(), "image-cache.json");
    
    private struct CacheEntry
    {
        [JsonProperty]
        public string FileName { get; set; }
        [JsonProperty]
        public long ExpiresAfter { get; set; }

        public bool Expired => DateTimeOffset.Now.ToUnixTimeMilliseconds() > ExpiresAfter;
    }
}
