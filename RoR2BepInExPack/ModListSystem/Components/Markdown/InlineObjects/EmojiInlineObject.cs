using Markdig.Extensions.Emoji;
using Markdig.Syntax.Inlines;
using RoR2BepInExPack.ModListSystem.Markdown;

namespace RoR2BepInExPack.ModListSystem.Components.Markdown.InlineObjects;

public class EmojiInlineObject : LiteralInlineObject
{
    public override void Parse(Inline inline, RenderContext renderCtx, InlineContext inlineCtx)
    {
        if (!label || !layoutElement || !emojiPrefab)
            return;
        
        if (inline is not EmojiInline emojiInline)
            return;
        
        label.fontSize = inlineCtx.FontSize;
        
        SetText(emojiInline.Content.Text, renderCtx, inlineCtx);
        
        inlineCtx.SetLineHeightIfBigger(inlineCtx.FontSize);
    }
}
