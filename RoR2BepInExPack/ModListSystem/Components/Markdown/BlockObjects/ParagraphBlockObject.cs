using Markdig.Syntax;
using RoR2BepInExPack.ModListSystem.Markdown;
using UnityEngine;

namespace RoR2BepInExPack.ModListSystem.Components.Markdown.BlockObjects;

public class ParagraphBlockObject : BaseMarkdownBlockObject
{
    public override void Parse(Block block, RenderContext renderCtx)
    {
        if (block is not ParagraphBlock paragraphBlock)
            return;

        if (paragraphBlock.Inline is null)
            return;

        AnchoredPosition = new Vector2(renderCtx.XPos, -renderCtx.YPos);

        var inlineCtx = new InlineContext
        {
            FontSize = renderCtx.FontSize,
            Styling = renderCtx.Styling
        };

        var currentYPos = renderCtx.YPos;

        foreach (var inline in paragraphBlock.Inline)
        {
            inlineCtx.LastItem = inline.NextSibling is null;
            renderCtx.InlineParser.Parse(inline, RectTransform, this, renderCtx, inlineCtx);
        }
        
        renderCtx.YPos += inlineCtx.YPos + inlineCtx.LineHeight;

        Height = renderCtx.YPos - currentYPos;
        
        preferredWidth = inlineCtx.PreferredWidth;
        minHeight = Height;
        preferredHeight = Height;

        // Bottom padding
        renderCtx.YPos += 16f;
    }
}
