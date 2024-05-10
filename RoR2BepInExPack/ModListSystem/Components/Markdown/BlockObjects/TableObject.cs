using Markdig.Extensions.Tables;
using Markdig.Syntax;
using RoR2BepInExPack.ModListSystem.Markdown;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2BepInExPack.ModListSystem.Components.Markdown.BlockObjects;

public class TableObject : BaseMarkdownBlockObject
{
    public override void Parse(Block block, RenderContext renderCtx)
    {
        if (block is not Table table)
            return;
        
        RectTransform.anchoredPosition = new Vector2(renderCtx.XPos, -renderCtx.YPos);

        var subCtx = new RenderContext(renderCtx) { YPos = 0, XPos = 0 };

        foreach (var tableRow in table)
        {
            renderCtx.BlockParser.Parse(tableRow, RectTransform, subCtx);
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(RectTransform);
        renderCtx.YPos += Height;
    }
}
