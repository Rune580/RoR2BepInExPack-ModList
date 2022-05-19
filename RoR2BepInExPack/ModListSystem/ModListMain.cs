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
    internal static Dictionary<string, ModDataInfo> guidToModDataInfo = new Dictionary<string, ModDataInfo>();
    private static Dictionary<string, GameObject> guidToPrefab = new Dictionary<string, GameObject>();
    internal static void Init()
    {
        ModListContent.Init();
        MenuModifications.InitHooks();
    }

    internal static void Start()
    {
        CreateModDataFromSerializableModDatas();
        PopulateDictionary();
    }

    private static void CreateModDataFromSerializableModDatas()
    {
        // SerializableModData.instances.ForEach(smd => smd.CreateModData());
    }

    private static void PopulateDictionary()
    {
        foreach (KeyValuePair<string, PluginInfo> kvp in Chainloader.PluginInfos)
        {
            string guid = kvp.Key;
            PluginInfo info = kvp.Value;

            //check for prefab first
            if (guidToPrefab.TryGetValue(info.Metadata.GUID, out GameObject prefab))
            {
                guidToModDataInfo.Add(info.Metadata.GUID, new ModDataInfo(info, prefab));
                continue;
            }
            //If no prefab exists, check for ModData
            ModData potentialModData = ModData.Instances.FirstOrDefault(md => md.Guid.Equals(guid, StringComparison.OrdinalIgnoreCase));
            if (potentialModData != null)
            {
                guidToModDataInfo[guid] = new ModDataInfo(info, potentialModData);
            }
            else //No ModData exists? Create a generic ModData
            {
                Log.Debug($"Could not find a ModData or UIPrefab for guid {guid}");

                ModData modData = new ModDataBuilder().WithBepInPlugin(info.Metadata).InternalBuild();
                guidToModDataInfo[guid] = new ModDataInfo(info, modData);
            }
        }
    }
    public static void AddCustomUIPrefab(string guid, GameObject uiPrefab) => guidToPrefab.Add(guid, uiPrefab);
}
