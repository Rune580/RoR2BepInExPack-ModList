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

        var tag = GetTag(emphasisInline.DelimiterChar, emphasisInline.DelimiterCount);
        var styling = inlineCtx.Styling;

        var currentYPos = inlineCtx.YPos;
        
        foreach (var subInline in emphasisInline)
        {
            inlineCtx.LastItem = emphasisInline.NextSibling is null && subInline.NextSibling is null;
            
            styling.AddStyleTags(tag);
            
            renderCtx.InlineParser.Parse(subInline, RectTransform, renderCtx, inlineCtx);

            styling.RemoveStyleTags(tag);
        }

        inlineCtx.LastItem = false;

        Height = inlineCtx.YPos - currentYPos;
    }

    private static string GetTag(char delimiterChar, int count)
    {
        var tag = delimiterChar switch
        {
            '*' => "b",
            '_' => "i",
            '~' => count == 2 ? "s" : "sub",
            '^' => "sup",
            '+' => "mark=#ffff0088",
            '=' => "u",
            _ =>  ""
        };

        if (string.IsNullOrEmpty(tag))
            Debug.Log($"Unhandled Emphasis: {delimiterChar}");

        return tag;
    }
}
