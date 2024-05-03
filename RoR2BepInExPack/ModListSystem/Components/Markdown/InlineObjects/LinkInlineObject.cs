using Markdig.Syntax.Inlines;

namespace RoR2BepInExPack.ModListSystem.Components.Markdown.InlineObjects;

public class LinkInlineObject : BaseMarkdownInlineObject
{
    public override void Parse(Inline inline, RenderContext renderCtx, InlineContext inlineCtx)
    {
        if (inline is not LinkInline linkInline)
            return;
    }
}
