using Markdig.Syntax;
using UnityEngine;

namespace RoR2BepInExPack.ModListSystem.Components.Markdown.BlockObjects;

public class FencedCodeBlockObject : CodeBlockObject
{
    public override void Parse(Block block, RenderContext renderCtx)
    {
        if (!verticalLayout || !linePrefab)
            return;
        
        if (block is not FencedCodeBlock fencedCodeBlock)
            return;

        var rt = GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0, -renderCtx.YPos);

        var codeLines = fencedCodeBlock.Lines.Lines;
        int emptyLines = 0;
        
        foreach (var code in codeLines)
        {
            if (string.IsNullOrEmpty(code.ToString()))
            {
                emptyLines++;
                continue;
            }

            for (int i = 0; i < emptyLines; i++)
                AddLine(" ", renderCtx);
            
            emptyLines = 0;
            
            AddLine(code, renderCtx);
        }

        renderCtx.YPos += verticalLayout.padding.vertical;
        
        // Bottom padding
        renderCtx.YPos += 16f;
    }
    
    
}
