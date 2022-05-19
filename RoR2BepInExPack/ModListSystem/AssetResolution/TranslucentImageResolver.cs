using LeTai.Asset.TranslucentImage;
using UnityEngine;

namespace RoR2BepInExPack.ModListSystem.AssetResolution;

public class TranslucentImageResolver : AssetResolver<Material>
{
    protected override void Resolve()
    {
        var translucentImage = GetComponent<TranslucentImage>();
        translucentImage.material = FetchAsset();
        translucentImage.enabled = true;
    }
}
