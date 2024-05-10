using Markdig.Extensions.Tables;
using Markdig.Syntax;
using RoR2BepInExPack.ModListSystem.Markdown;
using UnityEngine;

namespace RoR2BepInExPack.ModListSystem.Components.Markdown.BlockObjects;

public class TableRowObject : BaseMarkdownBlockObject
{
    public override void Parse(Block block, RenderContext renderCtx)
    {
        if (block is not TableRow tableRow)
            return;
        
        if (tableRow.Parent is not Table table)
            return;
        
        RectTransform.anchoredPosition = new Vector2(renderCtx.XPos, -renderCtx.YPos);
        
        var subCtx = new RenderContext(renderCtx) { YPos = 0, XPos = 0 };

        foreach (var tableCell in tableRow)
        {
            renderCtx.BlockParser.Parse(tableCell, RectTransform, subCtx);
        }
    }
}
