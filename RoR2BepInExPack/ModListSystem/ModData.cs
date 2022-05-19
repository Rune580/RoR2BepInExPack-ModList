using System;
using System.Collections.Generic;
using BepInEx;
using UnityEngine;

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
}
