using Markdig.Syntax.Inlines;
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

        foreach (var subInline in emphasisInline)
        {
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
    }
}
