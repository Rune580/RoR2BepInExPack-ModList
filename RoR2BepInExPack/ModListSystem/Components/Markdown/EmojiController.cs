using RoR2BepInExPack.ModListSystem.Markdown;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoR2BepInExPack.ModListSystem.Components.Markdown;

[RequireComponent(typeof(RawImage), typeof(RectTransform))]
public class EmojiController : UIBehaviour
{
    public RawImage targetImage;
    public RectTransform rt;
    public EmojiSet emojiSet;

    public void SetEmoji(string codePoint, float fontSize)
    {
        if (!emojiSet.TryGetEmoji(codePoint, out var emojiGraphic))
            return;

        if (!emojiGraphic.Texture)
            return;

        emojiGraphic.QualityScaleFactor = 1 + Mathf.FloorToInt(fontSize / 16);

        targetImage.texture = emojiGraphic.Texture;
    }
}
