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

        var rt = GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0, -renderCtx.YPos);
        
        foreach (var code in codeBlock.Lines.Lines)
        {
            AddLine(code, renderCtx);
        }

        renderCtx.YPos += verticalLayout.preferredHeight;
        
        // Bottom padding
        renderCtx.YPos += 16f;
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
        
        renderCtx.YPos += lineLabel.preferredHeight + verticalLayout.spacing;
    }
}
