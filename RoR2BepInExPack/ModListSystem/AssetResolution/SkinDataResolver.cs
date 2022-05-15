using RoR2.UI;
using RoR2.UI.SkinControllers;

namespace RoR2BepInExPack.ModListSystem.AssetResolution;

public class SkinDataResolver : AssetResolver<UISkinData>
{
    protected override void Resolve()
    {
        var controller = GetComponent<BaseSkinController>();
        controller.skinData = FetchAsset();
    }
}
