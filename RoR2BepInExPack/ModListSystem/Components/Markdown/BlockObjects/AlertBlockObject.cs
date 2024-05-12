using Markdig.Extensions.Alerts;
using Markdig.Syntax;
using RoR2.UI;
using RoR2BepInExPack.ModListSystem.Markdown;
using UnityEngine;

namespace RoR2BepInExPack.ModListSystem.Components.Markdown.BlockObjects;

public class AlertBlockObject : QuoteBlockObject
{
    public HGTextMeshProUGUI blockLabel;
    
    public override void Parse(Block block, RenderContext renderCtx)
    {
        if (!blockLabel || !blockLine || !content)
            return;
        
        if (block is not AlertBlock alertBlock)
            return;

        AnchoredPosition = new Vector2(renderCtx.XPos, -renderCtx.YPos);

        blockLabel.fontSize = renderCtx.FontSize * 1.25f;

        var kindString = alertBlock.Kind.ToString();
        
        Color color = kindString switch
        {
            "NOTE" => new Color(31 / 255f, 111 / 255f, 235 / 255f),
            "TIP" => new Color(35 / 255f, 134 / 255f, 55 / 255f),
            "IMPORTANT" => new Color(137 / 255f, 87 / 255f, 229 / 255f),
            "WARNING" => new Color(158 / 255f, 106 / 255f, 3 / 255f),
            "CAUTION" => new Color(218 / 255f, 54 / 255f, 51 / 255f),
            _ => Color.white
        };

        if (!string.IsNullOrEmpty(kindString))
        {
            blockLine.color = color;
            blockLabel.color = color;

            blockLabel.rectTransform.anchoredPosition = new Vector2(renderCtx.FontSize, 0f);
            
            blockLabel.SetText($"{kindString[0]}{kindString.Substring(1).ToLower()}");
        }
        
        var subCtx = new RenderContext(renderCtx)
        {
            XPos = renderCtx.XPos + renderCtx.FontSize,
            YPos = blockLabel.preferredHeight + renderCtx.FontSize
        };
        
        subCtx.ViewportRect = new Rect(renderCtx.ViewportRect.min, renderCtx.ViewportRect.size - new Vector2(subCtx.XPos, 0));

        foreach (var subBlock in alertBlock)
        {
            renderCtx.BlockParser.Parse(subBlock, content, subCtx);
        }

        Height = subCtx.YPos;

        renderCtx.YPos += subCtx.YPos;
        
        // Bottom padding
        renderCtx.YPos += 16f;
    }
}
