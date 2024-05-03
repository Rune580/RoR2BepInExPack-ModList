using RoR2BepInExPack.ModListSystem.Markdown;
using UnityEngine;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace RoR2BepInExPack.ModListSystem.Components.Markdown;

public class RenderContext
{
    public MarkdownBlockParser BlockParser;
    public MarkdownInlineParser InlineParser;

    public Rect ViewportRect;
    public float FontSize;
    public float YPos;
}
