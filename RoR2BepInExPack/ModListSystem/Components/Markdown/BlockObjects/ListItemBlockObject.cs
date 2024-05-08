using Markdig.Syntax;
using RoR2.UI;
using RoR2BepInExPack.ModListSystem.Markdown;
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
            // Todo
            Debug.LogWarning("Ordered lists aren't fully supported yet!");
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
        
        var subCtx = new RenderContext(renderCtx)
        {
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
