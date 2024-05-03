using Markdig.Syntax;
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

        var rt = GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0, -renderCtx.YPos);

        var inlineCtx = new InlineContext
        {
            FontSize = renderCtx.FontSize
        };

        foreach (var inline in paragraphBlock.Inline)
        {
            inlineCtx.LastItem = inline.NextSibling is null;
            renderCtx.InlineParser.Parse(inline, rt, renderCtx, inlineCtx);
        }
        
        renderCtx.YPos += inlineCtx.YPos;

        // Bottom padding
        renderCtx.YPos += 16f;
    }
}
