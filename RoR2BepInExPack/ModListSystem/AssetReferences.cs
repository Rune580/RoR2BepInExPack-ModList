using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using RoR2.UI;
using RoR2BepInExPack.ModListSystem.AssetResolution;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RoR2BepInExPack.ModListSystem;

internal static class AssetReferences
{
    private static readonly Dictionary<string, UnityObject> Assets = new();
    private static bool _initialized;

    internal static GameObject SplitButton;

    internal static void Init()
    {
        if (_initialized)
            throw new Exception("AssetReferences has already been initialized!");
        
        LoadSpriteReferences();
        LoadSkinReferences();
        LoadPrefabs();

        _initialized = true;
    }

    internal static T Fetch<T>(string key) where T : UnityObject
    {
        return Assets[key] as T;
    }

    private static async void LoadSpriteReferences()
    {
        Assets["texUICleanButton"] = Sprite.Create(await LoadAssetAsync<Texture2D>("RoR2/Base/UI/texUICleanButton.png"), new Rect(0, 0, 256, 64), new Vector2(128, 32), 100, 0, SpriteMeshType.Tight, new Vector4(8, 8, 8, 8));
        Assets["texUIOutlineOnly"] = Sprite.Create(await LoadAssetAsync<Texture2D>("RoR2/Base/UI/texUIOutlineOnly.png"), new Rect(0, 0, 256, 64), new Vector2(128, 32), 100, 0, SpriteMeshType.Tight, new Vector4(4, 4, 4, 4));
        Assets["texUIHighlightBoxOutlineThick"] = Sprite.Create(await LoadAssetAsync<Texture2D>("RoR2/Base/UI/texUIHighlightBoxOutlineThick.png"), new Rect(0, 0, 256, 64), new Vector2(128, 32), 100, 0, SpriteMeshType.Tight, new Vector4(6, 12, 12, 6));
    }

    private static async void LoadSkinReferences()
    {
        Assets["skinCleanButton"] = await LoadAssetAsync<UISkinData>("RoR2/Base/UI/skinCleanButton.asset");
    }

    private static void LoadPrefabs()
    {
        using var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("RoR2BepInExPack.modlistassets");
        var bundle = AssetBundle.LoadFromStream(assetStream);

        SplitButton = bundle.LoadAndResolvePrefab("SplitButton.prefab");
    }

    private static GameObject LoadAndResolvePrefab(this AssetBundle bundle, string assetName)
    {
        GameObject asset = bundle.LoadAsset<GameObject>($"Assets/ModListSystem/{assetName}");

        foreach (var resolver in asset.GetComponentsInChildren<BaseAssetResolver>())
            resolver.ResolveAsset();

        return asset;
    }

    private static async Task<TAsset> LoadAssetAsync<TAsset>(string path)
    {
        return await Addressables.LoadAssetAsync<TAsset>(path).Task;
    }
}
