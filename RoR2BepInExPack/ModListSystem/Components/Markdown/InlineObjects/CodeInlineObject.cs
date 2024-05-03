using Markdig.Syntax.Inlines;
using RoR2.UI;
using UnityEngine;

namespace RoR2BepInExPack.ModListSystem.Components.Markdown.InlineObjects;

public class CodeInlineObject : LiteralInlineObject
{
    public HGTextMeshProUGUI markLabel;
    public RectTransform markBg;
    
    public override void Parse(Inline inline, RenderContext renderCtx, InlineContext inlineCtx)
    {
        if (!label || !markLabel || !markBg)
            return;

        if (inline is not CodeInline codeInline)
            return;

        label.fontSize = inlineCtx.FontSize;
        markLabel.fontSize = inlineCtx.FontSize;

        var text = codeInline.Content;

        var xPos = inlineCtx.XPos;
        markLabel.SetText($"<mark=#252525 padding=100,100,100,100>{text}</mark>");

        var bgSize = markBg.sizeDelta;
        bgSize.x = markLabel.preferredWidth + 8;
        markBg.sizeDelta = bgSize;

        var bgPos = markBg.anchoredPosition;
        bgPos.x += xPos - 3;
        markBg.anchoredPosition = bgPos;
        
        SetText(text, renderCtx, inlineCtx);
        
        Width = label.preferredWidth;
        Height = label.preferredHeight;
    }
}
