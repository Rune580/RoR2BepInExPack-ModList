using Markdig.Syntax.Inlines;
using RoR2.UI;
using RoR2BepInExPack.ModListSystem.Markdown;
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
        
        SetText(text, renderCtx, inlineCtx);
        markLabel.SetText($"<mark=#252525 padding=5,5,5,5>{label.text}</mark>");
        
        Width = label.preferredWidth;
        Height = label.preferredHeight;
    }
}
