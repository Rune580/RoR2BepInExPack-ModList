using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Bootstrap;
using RoR2;
using UnityEngine;

namespace RoR2BepInExPack.ModListSystem;

public class ModDataInfo
{
    public PluginInfo PluginInfo { get; }
    public ModData ModData { get;}
    public GameObject UIPrefab { get; }

    public bool hasCustomPrefab { get; }

    public ModDataInfo(PluginInfo info, ModData modData)
    {
        PluginInfo = info;
        ModData = modData;
        UIPrefab = null;
        hasCustomPrefab = false;
    }

    public ModDataInfo(PluginInfo info, GameObject uiPrefab)
    {
        PluginInfo = info;
        ModData = null;
        UIPrefab = uiPrefab;
        hasCustomPrefab = true;
    }
}

public static class ModListMain
{
    private static Dictionary<string, ModDataInfo> guidToModDataInfo = new Dictionary<string, ModDataInfo>();
    private static List<Tuple<string, GameObject>> guidCustomUIPairs = new List<Tuple<string, GameObject>>();
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

            //Check for prefab first
            Tuple<string, GameObject> matchingTuple = guidCustomUIPairs.FirstOrDefault(x => x.Item1.Equals(guid, StringComparison.OrdinalIgnoreCase));
            if(matchingTuple != null)
            {
                guidToModDataInfo[guid] = new ModDataInfo(info, matchingTuple.Item2);
                continue;
            }

            //If no prefab exists, check for ModData
            ModData potentialModData = ModData.instances.FirstOrDefault(md => md.modGUIDIdentifier.Equals(guid, StringComparison.OrdinalIgnoreCase));
            if(potentialModData)
            {
                guidToModDataInfo[guid] = new ModDataInfo(info, potentialModData);
            }
            else //No ModData exists? Create a generic ModData
            {
                Log.Debug($"Could not find a ModData for guid {guid}");
                guidToModDataInfo[guid] = new ModDataInfo(info, ModData.CreateGeneric(info));
            }
        }
    }

    public static void AddCustomUIPrefab(string guid, GameObject uiPrefab) => guidCustomUIPairs.Add(new Tuple<string, GameObject>(guid, uiPrefab));
}
