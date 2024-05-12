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
        
        AnchoredPosition = new Vector2(renderCtx.XPos, -renderCtx.YPos);

        var subCtx = new RenderContext(renderCtx) { YPos = 0, XPos = 0 };

        TableCellObject[,] columnCells = null;

        var enableBg = true;
        
        // First pass - Create table with minimal size constraints.
        for (int row = 0; row < table.Count; row++)
        {
            var subBlock = table[row];
            var blockObject = renderCtx.BlockParser.Parse(subBlock, RectTransform, subCtx);
            if (blockObject is not TableRowObject rowObject)
                continue;

            var tableRow = (TableRow)subBlock;
            if (tableRow.IsHeader)
                rowObject.background.enabled = true;

            if (enableBg && !tableRow.IsHeader)
            {
                rowObject.background.enabled = true;
                enableBg = false;
            }
            else if (!enableBg)
            {
                rowObject.background.enabled = false;
                enableBg = true;
            }

            if (row + 1 >= table.Count)
                rowObject.line.enabled = false;

            columnCells ??= new TableCellObject[rowObject.Cells.Length, table.Count];

            for (int column = 0; column < rowObject.Cells.Length; column++)
            {
                columnCells[column, row] = rowObject.Cells[column];
            }
        }

        if (columnCells is null)
            return;

        LayoutRebuilder.ForceRebuildLayoutImmediate(RectTransform);

        var columns = columnCells.GetLength(0);
        var rows = columnCells.GetLength(1);

        var columnWidths = new float[columns];

        var maxWidth = 0f;
        for (int column = 0; column < columns; column++)
        {
            // Determine the max size of the column
            var maxColumnWidth = 0f;
            for (int row = 0; row < rows; row++)
            {
                var cell = columnCells[column, row];
                
                maxColumnWidth = Mathf.Max(maxColumnWidth, cell.preferredWidth);
            }

            columnWidths[column] = maxColumnWidth;
            maxWidth += maxColumnWidth;
        }

        var widthScale = Mathf.Clamp(renderCtx.ViewportRect.width / maxWidth, 0, 1);
        
        for (int column = 0; column < columns; column++)
        {
            var maxColWidth = columnWidths[column] * widthScale;
            var colSize = new Vector2(maxColWidth, renderCtx.ViewportRect.height);
            var colCtx = new RenderContext(renderCtx) { ViewportRect = new Rect(Vector2.zero, colSize) };
            
            for (int row = 0; row < rows; row++)
                columnCells[column, row].Rebuild(colCtx);
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(RectTransform);
        
        renderCtx.YPos += Height;
        
        // Bottom padding
        renderCtx.YPos += 16f;
    }
}
