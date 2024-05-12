using Markdig.Syntax;
using RoR2BepInExPack.ModListSystem.Markdown;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2BepInExPack.ModListSystem.Components.Markdown.BlockObjects;

public class QuoteBlockObject : BaseMarkdownBlockObject
{
    public Image blockLine;
    public RectTransform content;
    
    public override void Parse(Block block, RenderContext renderCtx)
    {
        if (!blockLine || !content)
            return;
        
        if (block is not QuoteBlock quoteBlock)
            return;
        
        AnchoredPosition = new Vector2(renderCtx.XPos, -renderCtx.YPos);
        
        var subCtx = new RenderContext(renderCtx)
        {
            XPos = renderCtx.XPos + renderCtx.FontSize,
            YPos = renderCtx.FontSize
        };
        
        subCtx.ViewportRect = new Rect(renderCtx.ViewportRect.min, renderCtx.ViewportRect.size - new Vector2(subCtx.XPos, 0));
        
        subCtx.Styling.AddStyleTags("color=#9f9f9f");
        
        foreach (var subBlock in quoteBlock)
            renderCtx.BlockParser.Parse(subBlock, content, subCtx);
        
        subCtx.Styling.RemoveStyleTags("color=#9f9f9f");

        Height = subCtx.YPos;

        renderCtx.YPos += subCtx.YPos;
        
        // Bottom padding
        renderCtx.YPos += 16f;
    }
}
