using System.Collections.Generic;
using Markdig.Extensions.Tables;
using Markdig.Syntax;
using RoR2BepInExPack.ModListSystem.Markdown;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2BepInExPack.ModListSystem.Components.Markdown.BlockObjects;

public class TableCellObject : BaseMarkdownBlockObject
{
    private TableCell _data;
    private BaseMarkdownBlockObject[] _childBlocks;

    public Image line;
    
    public override void Parse(Block block, RenderContext renderCtx)
    {
        if (block is not TableCell tableCell)
            return;

        _data = tableCell;

        if (tableCell.Parent is not TableRow tableRow)
            return;

        if (tableRow.Parent is not Table table)
            return;
        
        AnchoredPosition = new Vector2(renderCtx.XPos, -renderCtx.YPos);
        
        var subCtx = new RenderContext(renderCtx)
        {
            YPos = renderCtx.FontSize / 2f,
            XPos = renderCtx.FontSize / 2f,
            ViewportRect = new Rect(renderCtx.ViewportRect.position, renderCtx.ViewportRect.size - new Vector2(renderCtx.FontSize / 2f, 0))
        };

        var childBlocks = new List<BaseMarkdownBlockObject>();
        
        foreach (var subBlock in tableCell)
        {
            var blockObject = renderCtx.BlockParser.Parse(subBlock, RectTransform, subCtx);

            preferredWidth = Mathf.Min(Mathf.Max(preferredWidth, blockObject.preferredWidth + renderCtx.FontSize), subCtx.ViewportRect.width);
            preferredHeight = Mathf.Max(preferredHeight, blockObject.preferredHeight + renderCtx.FontSize);
            
            childBlocks.Add(blockObject);
        }

        _childBlocks = childBlocks.ToArray();
    }

    public void Rebuild(RenderContext renderCtx)
    {
        foreach (var childBlock in _childBlocks)
            DestroyImmediate(childBlock.gameObject);

        minWidth = renderCtx.ViewportRect.width;
        preferredWidth = minWidth;
        preferredHeight = 0;
        
        Parse(_data, renderCtx);
    }
}
