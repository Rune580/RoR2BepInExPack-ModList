using Markdig.Syntax;
using UnityEngine;

namespace RoR2BepInExPack.ModListSystem.Components.Markdown.BlockObjects;

public class HeadingBlockObject : BaseMarkdownBlockObject
{
    public GameObject line;
    
    public override void Parse(Block block, RenderContext renderCtx)
    {
        if (block is not HeadingBlock headingBlock)
            return;

        if (headingBlock.Inline is null)
            return;

        RectTransform.anchoredPosition = new Vector2(renderCtx.XPos, -renderCtx.YPos);
        
        var inlineCtx = new InlineContext
        {
            FontSize = renderCtx.FontSize * (1 + (1f / headingBlock.Level))
        };

        var currentYPos = renderCtx.YPos;

        foreach (var inline in headingBlock.Inline)
        {
            inlineCtx.LastItem = inline.NextSibling is null;
            renderCtx.InlineParser.Parse(inline, RectTransform, renderCtx, inlineCtx);
        }

        renderCtx.YPos += inlineCtx.YPos + renderCtx.FontSize * 0.3f;;
        
        Height += renderCtx.YPos - currentYPos;
        
        // Add bottom padding
        renderCtx.YPos += renderCtx.FontSize * 0.75f;
        
        line.SetActive(headingBlock.Level < 3);
    }
}
