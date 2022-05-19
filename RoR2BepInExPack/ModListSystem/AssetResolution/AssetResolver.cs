using RoR2BepInExPack.ModListSystem.AssetResolution;

namespace RoR2BepInExPack.ModListSystem.AssetResolution;

public abstract class AssetResolver<TAsset> : BaseAssetResolver where TAsset : UnityObject
{
    protected TAsset FetchAsset()
    {
        return AssetReferences.Fetch<TAsset>(key);
    }
}
