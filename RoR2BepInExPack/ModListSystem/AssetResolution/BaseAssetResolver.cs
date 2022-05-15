using UnityEngine;

namespace RoR2BepInExPack.ModListSystem.AssetResolution;

public abstract class BaseAssetResolver : MonoBehaviour
{
    public string key;

    private void Awake()
    {
        ResolveAsset();
    }

    private void OnEnable()
    {
        ResolveAsset();
    }

    internal void ResolveAsset()
    {
        Resolve();
        
        DestroyImmediate(this);
    }
    
    protected abstract void Resolve();
}
