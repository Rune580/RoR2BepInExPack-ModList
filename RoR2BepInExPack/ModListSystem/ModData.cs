using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RoR2BepInExPack.ModListSystem;
public class ModData
{
    public string ModGUIDIdentifier { get; }
    public string ModDescription { get; }
    public Sprite ModIcon { get; }
    public HyperLink[] Links { get; }

    internal readonly static List<ModData> instances = new List<ModData>();

    public ModData(string guid, string description, Sprite icon, params HyperLink[] hyperlinks)
    {
        ModGUIDIdentifier = guid;
        ModDescription = description;
        ModIcon = icon;
        Links = hyperlinks;

        instances.Add(this);
    }

    internal static ModData CreateGeneric(string guid)
    {
        var genericModData = new ModData(guid, $"No ModData Provided...", null, Array.Empty<HyperLink>());
        instances.Remove(genericModData);
        return genericModData;
    }
}

[Serializable]
public struct HyperLink
{
    public string displayName;
    public string link;

    public HyperLink(string displayName, string link)
    {
        this.displayName = displayName;
        this.link = link;
    }
}
