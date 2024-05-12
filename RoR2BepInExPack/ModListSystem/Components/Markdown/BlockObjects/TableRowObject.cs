using System.Collections.Generic;
using Markdig.Extensions.Tables;
using Markdig.Syntax;
using RoR2BepInExPack.ModListSystem.Markdown;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2BepInExPack.ModListSystem.Components.Markdown.BlockObjects;

public class TableRowObject : BaseMarkdownBlockObject
{
    public TableCellObject[] Cells { get; private set; }

    public Image background;
    public Image line;
    
    public override void Parse(Block block, RenderContext renderCtx)
    {
        if (!background)
            return;
        
        if (block is not TableRow tableRow)
            return;
        
        if (tableRow.Parent is not Table table)
            return;
        
        AnchoredPosition = new Vector2(renderCtx.XPos, -renderCtx.YPos);
        
        var subCtx = new RenderContext(renderCtx) { YPos = 0, XPos = 0 };

        var cells = new List<TableCellObject>();

        for (var column = 0; column < tableRow.Count; column++)
        {
            var tableCell = tableRow[column];
            var blockObject = renderCtx.BlockParser.Parse(tableCell, RectTransform, subCtx);

            if (blockObject is not TableCellObject cellObject)
                continue;

            if (column + 1 >= tableRow.Count)
                cellObject.line.enabled = false;

            cells.Add(cellObject);
        }

        Cells = cells.ToArray();
    }
}
