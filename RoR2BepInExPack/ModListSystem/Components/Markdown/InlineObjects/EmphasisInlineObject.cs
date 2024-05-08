using Markdig.Syntax.Inlines;
using RoR2BepInExPack.ModListSystem.Markdown;
using UnityEngine;

namespace RoR2BepInExPack.ModListSystem.Components.Markdown.InlineObjects;

public class EmphasisInlineObject : BaseMarkdownInlineObject
{
    public override void Parse(Inline inline, RenderContext renderCtx, InlineContext inlineCtx)
    {
        if (inline is not EmphasisInline emphasisInline)
            return;

        var bold = emphasisInline.DelimiterChar == '*';
        var italicize = emphasisInline.DelimiterChar == '_';

        if (!bold && !italicize)
        {
            Debug.Log($"Unhandled Emphasis: {emphasisInline.DelimiterChar}");
        }

        var currentYPos = inlineCtx.YPos;
        
        foreach (var subInline in emphasisInline)
        {
            inlineCtx.LastItem = emphasisInline.NextSibling is null && subInline.NextSibling is null;
            
            if (bold)
                inlineCtx.AddStyleTags("b");
            else if (italicize)
                inlineCtx.AddStyleTags("i");
            
            renderCtx.InlineParser.Parse(subInline, RectTransform, renderCtx, inlineCtx);

            if (bold)
                inlineCtx.RemoveStyleTags("b");
            else if (italicize)
                inlineCtx.RemoveStyleTags("i");
        }

        inlineCtx.LastItem = false;

        Height = inlineCtx.YPos - currentYPos;
    }
}
