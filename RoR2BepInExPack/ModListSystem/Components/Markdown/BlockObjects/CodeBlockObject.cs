using Markdig.Helpers;
using Markdig.Syntax;
using RoR2.UI;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2BepInExPack.ModListSystem.Components.Markdown.BlockObjects;

public class CodeBlockObject : BaseMarkdownBlockObject
{
    public VerticalLayoutGroup verticalLayout;
    public GameObject linePrefab;
    
    public override void Parse(Block block, RenderContext renderCtx)
    {
        if (!verticalLayout || !linePrefab)
            return;
        
        if (block is not CodeBlock codeBlock)
            return;

        RectTransform.anchoredPosition = new Vector2(renderCtx.XPos, -renderCtx.YPos);
        
        var codeLines = codeBlock.Lines.Lines;
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

        LayoutRebuilder.ForceRebuildLayoutImmediate(RectTransform);
        renderCtx.YPos += verticalLayout.preferredHeight;
        
        // Bottom padding
        renderCtx.YPos += renderCtx.FontSize;
    }

    protected void AddLine(StringLine stringLine, RenderContext renderCtx) => AddLine(stringLine.ToString(), renderCtx);

    protected void AddLine(string text, RenderContext renderCtx)
    {
        var line = Instantiate(linePrefab, RectTransform);
        var lineLabel = line.GetComponent<HGTextMeshProUGUI>();
        if (!lineLabel)
            return;

        lineLabel.fontSize = renderCtx.FontSize;
        
        lineLabel.SetText(text);
        line.SetActive(true);
    }
}
