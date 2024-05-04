using Markdig.Syntax;
using RoR2.UI;
using UnityEngine;

namespace RoR2BepInExPack.ModListSystem.Components.Markdown.BlockObjects;

public class ListItemBlockObject : BaseMarkdownBlockObject
{
    public HGTextMeshProUGUI bulletLabel;
    public RectTransform content;
    
    public override void Parse(Block block, RenderContext renderCtx)
    {
        if (!bulletLabel || !content)
            return;
        
        if (block is not ListItemBlock listItemBlock)
            return;

        if (listItemBlock.Parent is not ListBlock listBlock)
            return;
        
        if (listBlock.IsOrdered)
        {
            
        }
        else
        {
            bulletLabel.fontSize = renderCtx.FontSize;
            bulletLabel.SetText("•");
            
            var bulletPos = bulletLabel.rectTransform.anchoredPosition;
            bulletPos.y = -((renderCtx.FontSize / 2f) + 1);
            bulletLabel.rectTransform.anchoredPosition = bulletPos;

            var bulletSize = bulletLabel.rectTransform.sizeDelta;
            bulletSize.x = renderCtx.FontSize;
            bulletLabel.rectTransform.sizeDelta = bulletSize;
        }
        
        RectTransform.anchoredPosition = new Vector2(renderCtx.XPos, -renderCtx.YPos);
        
        var subCtx = new RenderContext
        {
            BlockParser = renderCtx.BlockParser,
            InlineParser = renderCtx.InlineParser,
            ViewportRect = renderCtx.ViewportRect,
            FontSize = renderCtx.FontSize,
            XPos = renderCtx.XPos,
            YPos = 0
        };

        foreach (var subBlock in listItemBlock)
        {
            renderCtx.BlockParser.Parse(subBlock, content, subCtx);
        }
        
        Height = subCtx.YPos;
        renderCtx.YPos += subCtx.YPos;
    }
}
