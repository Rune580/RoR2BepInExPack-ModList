using UnityEngine;

namespace RoR2BepInExPack.ModListSystem.AssetResolution;

public abstract class AssetResolver<TAsset> : BaseAssetResolver where TAsset : Object
{
    protected TAsset FetchAsset()
    {
        return AssetReferences.Fetch<TAsset>(key);
    }
}
