using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Bootstrap;
using RoR2;

namespace RoR2BepInExPack.ModListSystem;

public class ModDataInfo
{
    public PluginInfo PluginInfo { get; }
    public ModData ModData { get; }

    public ModDataInfo(PluginInfo info, ModData modData)
    {
        PluginInfo = info;
        ModData = modData;
    }
}

public static class ModListMain
{
    private static Dictionary<string, ModDataInfo> guidToModDataInfo = new Dictionary<string, ModDataInfo>();
    internal static void Init()
    {
        ModListContent.Init();
        MenuModifications.InitHooks();
    }

    internal static void SetupDictionary()
    {
        foreach(KeyValuePair<string, PluginInfo> kvp in Chainloader.PluginInfos)
        {
            string guid = kvp.Key;
            PluginInfo info = kvp.Value;
            ModData potentialModData = ModData.instances.FirstOrDefault(md => md.modGUIDIdentifier.Equals(guid, StringComparison.OrdinalIgnoreCase));
            
            if(potentialModData)
            {
                guidToModDataInfo[guid] = new ModDataInfo(info, potentialModData);
            }
            else
            {
                Log.Debug($"Could not find a ModData for guid {guid}");
                guidToModDataInfo[guid] = new ModDataInfo(info, ModData.CreateGeneric(info));
            }
        }
    }
}
