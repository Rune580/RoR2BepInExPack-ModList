using System;
using System.Collections.Generic;
using System.Text;
using BepInEx;
using UnityEngine;

namespace RoR2BepInExPack;

[CreateAssetMenu(menuName = "RoR2BepinexPack/ModData")]

public class ModData : ScriptableObject
{
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
    [Tooltip($"The GUID of the mod this ModData belongs to.")]
    public string modGUIDIdentifier;
    public string modDescription;
    public Sprite modIcon;
    public HyperLink[] links;

    internal readonly static List<ModData> instances = new List<ModData>();

    private void OnEnable()
    {
        instances.Add(this);
    }
    private void OnDisable()
    {
        instances.Remove(this);
    }

    internal static ModData CreateGeneric(PluginInfo pluginInfo)
    {
        ModData data = CreateInstance<ModData>();
        data.modIcon = null;
        data.links = Array.Empty<HyperLink>();
        data.modGUIDIdentifier = pluginInfo.Metadata.GUID;
        data.modDescription = $"No ModData provided...";

        return data;
    }
}
