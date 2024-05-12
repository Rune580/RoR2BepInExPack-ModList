using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace RoR2BepInExPack.ModListSystem.Markdown;

public class InlineContext
{
    public float FontSize;

    public float XPos;
    public float YPos;
    public bool LastItem;

    public float LineHeight;
    public float PreferredWidth;

    public TextStyling Styling;
    
    public void SetLineHeightIfBigger(float lineHeight) => LineHeight = Mathf.Max(LineHeight, lineHeight);
    
    public void SetPreferredWidthIfBigger(float preferredWidth) => PreferredWidth = Mathf.Max(PreferredWidth, preferredWidth);
}
