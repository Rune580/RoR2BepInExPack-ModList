using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using RoR2;
using UnityEngine;
using Path = System.IO.Path;

namespace RoR2BepInExPack.ModListSystem;

public class ModData
{
    internal static readonly List<ModData> Instances = new();
    
    public string Guid { get; internal set; }
    public string Name { get; internal set; }
    public Version Version { get; internal set; }
    public string DescriptionToken { get; internal set; }
    public Sprite Icon { get; internal set; }
    public HyperLink[] Links { get; internal set; }
    public bool SupportsRuntimeToggling { get; internal set; }

    public ModData() { }

    internal void ValidateIcon(PluginInfo info)
    {
        if (Icon)
            return;

        try
        {
            string searchDir = Path.GetFullPath(info.Location);

            while (!string.Equals(Directory.GetParent(searchDir)!.Name, "plugins", StringComparison.OrdinalIgnoreCase))
                searchDir = Directory.GetParent(searchDir)!.FullName;

            string iconPath = Directory.EnumerateFiles(searchDir, "icon.png", SearchOption.AllDirectories).FirstOrDefault();

            if (iconPath == default)
                return;

            Texture2D texture = new Texture2D(256, 256);
            if (!texture.LoadImage(File.ReadAllBytes(iconPath)))
                return;

            if (!texture)
                return;
            
            Icon = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100);
        }
        catch
        {
            // Couldn't find icon for one reason or another
        }
    }
}
