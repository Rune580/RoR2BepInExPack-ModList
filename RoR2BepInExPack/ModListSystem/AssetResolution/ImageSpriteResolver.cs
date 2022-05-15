using UnityEngine;
using UnityEngine.UI;

namespace RoR2BepInExPack.ModListSystem.AssetResolution;

public class ImageSpriteResolver : AssetResolver<Sprite>
{
    protected override void Resolve()
    {
        var image = GetComponent<Image>();
        image.sprite = FetchAsset();
    }
}
