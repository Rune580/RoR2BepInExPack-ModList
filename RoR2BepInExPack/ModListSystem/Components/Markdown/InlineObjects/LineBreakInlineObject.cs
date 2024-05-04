using Markdig.Syntax.Inlines;

namespace RoR2BepInExPack.ModListSystem.Components.Markdown.InlineObjects;

public class LineBreakInlineObject : LiteralInlineObject
{
    public override void Parse(Inline inline, RenderContext renderCtx, InlineContext inlineCtx)
    {
        if (!label)
            return;

        if (inline is not LineBreakInline lineBreakInline)
            return;
        
        label.fontSize = inlineCtx.FontSize;

        if (lineBreakInline.PreviousSibling is not LineBreakInline)
        {
            SetText(" ", renderCtx, inlineCtx);
            return;
        }

        inlineCtx.XPos = 0;
        inlineCtx.YPos += TextHeight(" ");
    }
}
