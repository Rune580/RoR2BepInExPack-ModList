using UnityEngine;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace RoR2BepInExPack.ModListSystem.Markdown;

public class RenderContext
{
    public MarkdownBlockParser BlockParser;
    public MarkdownInlineParser InlineParser;

    public Rect ViewportRect;
    public float FontSize;
    public float XPos;
    public float YPos;

    public TextStyling Styling;

    public RenderContext()
    {
        Styling = new TextStyling();
    }

    public RenderContext(RenderContext other)
    {
        BlockParser = other.BlockParser;
        InlineParser = other.InlineParser;

        ViewportRect = other.ViewportRect;
        FontSize = other.FontSize;
        XPos = other.XPos;
        YPos = other.YPos;

        Styling = other.Styling;
    }
}
