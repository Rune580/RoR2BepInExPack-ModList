using UnityEngine;
using UnityEngine.UI;

namespace RoR2BepInExPack.ModListSystem.AssetResolution;

public class ImageMaterialResolver : AssetResolver<Material>
{
    protected override void Resolve()
    {
        var image = GetComponent<Image>();
        image.material = FetchAsset();
    }
}
