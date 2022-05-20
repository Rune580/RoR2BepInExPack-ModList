using System;
using System.Collections.Generic;
using System.IO;
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
    internal static GameObject MenuMods;

    internal static async Task Init()
    {
        if (_initialized)
            throw new Exception("AssetReferences has already been initialized!");

        await LoadSpriteReferences();
        await LoadSkinReferences();
        await LoadMaterialReferences();
        
        LoadPrefabs();

        _initialized = true;
    }

    internal static T Fetch<T>(string key) where T : UnityObject
    {
        return Assets[key] as T;
    }

    private static async Task LoadSpriteReferences()
    {
        Assets["texUICleanButton"] = Sprite.Create(await LoadAssetAsync<Texture2D>("RoR2/Base/UI/texUICleanButton.png"), new Rect(0, 0, 256, 64), new Vector2(128, 32), 100, 0, SpriteMeshType.Tight, new Vector4(8, 8, 8, 8));
        Assets["texUIOutlineOnly"] = Sprite.Create(await LoadAssetAsync<Texture2D>("RoR2/Base/UI/texUIOutlineOnly.png"), new Rect(0, 0, 256, 64), new Vector2(128, 32), 100, 0, SpriteMeshType.Tight, new Vector4(4, 4, 4, 4));
        Assets["texUIHighlightBoxOutlineThick"] = Sprite.Create(await LoadAssetAsync<Texture2D>("RoR2/Base/UI/texUIHighlightBoxOutlineThick.png"), new Rect(0, 0, 256, 64), new Vector2(128, 32), 100, 0, SpriteMeshType.Tight, new Vector4(6, 12, 12, 6));
        Assets["texUIHighlightBoxOutline"] = Sprite.Create(await LoadAssetAsync<Texture2D>("RoR2/Base/UI/texUIHighlightBoxOutline.png"), new Rect(0, 0, 256, 64), new Vector2(128, 32), 100, 0, SpriteMeshType.Tight, new Vector4(4, 4, 4, 4));
        Assets["texUIAnimateSliceNakedButton"] = Sprite.Create(await LoadAssetAsync<Texture2D>("RoR2/Base/UI/texUIAnimateSliceNakedButton.png"), new Rect(0, 0, 64, 64), new Vector2(32, 32), 100, 0, SpriteMeshType.Tight, new Vector4(8, 8, 8, 8));
        Assets["texUIPopupRect"] = Sprite.Create(await LoadAssetAsync<Texture2D>("RoR2/Base/UI/texUIPopupRect.png"), new Rect(0, 0, 128, 128), new Vector2(64, 64), 100, 0, SpriteMeshType.Tight, new Vector4(5, 5, 5, 5));
    }

    private static async Task LoadSkinReferences()
    {
        Assets["skinCleanButton"] = await LoadAssetAsync<UISkinData>("RoR2/Base/UI/skinCleanButton.asset");
        Assets["skinNakedButton"] = await LoadAssetAsync<UISkinData>("RoR2/Base/UI/skinNakedButton.asset");
    }

    private static async Task LoadMaterialReferences()
    {
        Assets["matUIAnimateAlphaNakedButton"] = await LoadAssetAsync<Material>("RoR2/Base/UI/matUIAnimateAlphaNakedButton.mat");
        Assets["Default-Translucent"] = await LoadAssetAsync<Material>("TranslucentImage/Default-Translucent.mat");
    }

    private static void LoadPrefabs()
    {
        var assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var bundle = AssetBundle.LoadFromFile($"{assemblyLocation}/modlistsystem-assets");

        SplitButton = bundle.LoadAndResolvePrefab("SplitButton.prefab");
        MenuMods = bundle.LoadAndResolvePrefab("MENU_ Mods.prefab");
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
