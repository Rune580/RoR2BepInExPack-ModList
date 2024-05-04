using Markdig.Syntax;
using UnityEngine;

namespace RoR2BepInExPack.ModListSystem.Components.Markdown.BlockObjects;

public class ListBlockObject : BaseMarkdownBlockObject
{
    public override void Parse(Block block, RenderContext renderCtx)
    {
        if (block is not ListBlock listBlock)
            return;
        
        RectTransform.anchoredPosition = new Vector2(renderCtx.XPos, -renderCtx.YPos);

        var subCtx = new RenderContext
        {
            BlockParser = renderCtx.BlockParser,
            InlineParser = renderCtx.InlineParser,
            ViewportRect = renderCtx.ViewportRect,
            FontSize = renderCtx.FontSize,
            XPos = renderCtx.XPos + renderCtx.FontSize,
            YPos = 0
        };
        
        foreach (var listItemBlock in listBlock)
        {
            renderCtx.BlockParser.Parse(listItemBlock, RectTransform, subCtx);
        }
        
        renderCtx.YPos += subCtx.YPos;
    }
}
