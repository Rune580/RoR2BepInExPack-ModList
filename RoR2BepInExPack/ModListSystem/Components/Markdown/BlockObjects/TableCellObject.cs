using Markdig.Extensions.Tables;
using Markdig.Syntax;
using RoR2BepInExPack.ModListSystem.Markdown;
using UnityEngine;

namespace RoR2BepInExPack.ModListSystem.Components.Markdown.BlockObjects;

public class TableCellObject : BaseMarkdownBlockObject
{
    public override void Parse(Block block, RenderContext renderCtx)
    {
        if (block is not TableCell tableCell)
            return;

        if (tableCell.Parent is not TableRow tableRow)
            return;

        if (tableRow.Parent is not Table table)
            return;
        
        RectTransform.anchoredPosition = new Vector2(renderCtx.XPos, -renderCtx.YPos);
        
        var subCtx = new RenderContext(renderCtx) { YPos = 0, XPos = 0 };
        
        foreach (var subBlock in tableCell)
        {
            var blockObject = renderCtx.BlockParser.Parse(subBlock, RectTransform, subCtx);

            preferredWidth = Mathf.Max(preferredWidth, blockObject.preferredWidth);
            preferredHeight = Mathf.Max(preferredHeight, blockObject.preferredHeight);
        }
    }
}
