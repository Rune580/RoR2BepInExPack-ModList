using System.Collections.Generic;
using System.Linq;
using Markdig.Syntax.Inlines;
using RoR2.UI;
using TMPro;

namespace RoR2BepInExPack.ModListSystem.Components.Markdown.InlineObjects;

public class LiteralInlineObject : BaseMarkdownInlineObject
{
    public HGTextMeshProUGUI label;
    
    public override void Parse(Inline inline, RenderContext renderCtx, InlineContext inlineCtx)
    {
        if (!label)
            return;

        if (inline is not LiteralInline literalInline)
            return;

        label.fontSize = inlineCtx.FontSize;
        
        var text = literalInline.ToString();

        SetText(text, renderCtx, inlineCtx);

        if (inline.NextSibling is LinkInline && text.EndsWith(" "))
            inlineCtx.XPos -= TextWidthApproximation(" ", FontStyles.Normal);
    }

    protected void SetText(string text, RenderContext renderCtx, InlineContext inlineCtx)
    {
        AnchoredYPos = -inlineCtx.YPos;

        var style = FontStyles.Normal;

        if (inlineCtx.HasTag("b"))
            style |= FontStyles.Bold;
        if (inlineCtx.HasTag("i"))
            style |= FontStyles.Italic;

        var lineWidths = TextWidthMultiLine(text, style, inlineCtx.XPos, renderCtx.ViewportRect.width, out var textPerLine);
        var textWithoutLastLine = string.Join(" ", textPerLine.AsEnumerable().Take(textPerLine.Length - 1));
        var height = TextHeight(textWithoutLastLine);

        label.SetText(!string.IsNullOrEmpty(inlineCtx.Styling)
            ? $"<line-indent={inlineCtx.XPos}px>{inlineCtx.Styling.Replace("{0}", text)}</line-indent>"
            : $"<line-indent={inlineCtx.XPos}px>{text}</line-indent>");

        inlineCtx.XPos = lineWidths.Last();

        if (lineWidths.Length > 1 && !inlineCtx.LastItem)
        {
            inlineCtx.YPos += height;
        }
        else if (inlineCtx.LastItem)
        {
            inlineCtx.YPos += label.preferredHeight;
        }
    }

    private float[] TextWidthMultiLine(string text, FontStyles style, float xOffset, float maxWidth, out string[] textPerLine)
    {
        var words = text.Split(' ');

        int line = 0;
        List<float> lineWidths = [xOffset];
        List<string> textPerLineList = [""];

        var spaceWidth = TextWidthApproximation(" ", FontStyles.Normal);

        foreach (var word in words)
        {
            var lineWidth = lineWidths[line];
            var lineText = textPerLineList[line];

            var wordWidth = TextWidthApproximation(word, style) + spaceWidth;
            
            if (lineWidth + wordWidth >= maxWidth)
            {
                lineWidth = 0;
                lineWidths.Add(lineWidth);

                if (lineText.Length > 0)
                {
                    textPerLineList[line] = lineText.Substring(0, lineText.Length - 1);
                    lineWidths[line] -= spaceWidth;
                }
                
                line++;
                
                textPerLineList.Add("");
                lineText = textPerLineList[line];
            }

            lineText += $"{word} ";

            lineWidth += wordWidth;
            lineWidths[line] = lineWidth;
            textPerLineList[line] = lineText;
        }

        if (!text.EndsWith(" "))
            lineWidths[line] -= spaceWidth;

        textPerLine = textPerLineList.ToArray();

        return lineWidths.ToArray();
    }
    
    private float TextWidthApproximation(string text, FontStyles style)
    {
        if (!label)
            return 0;
        
        var fontSize = label.fontSize;
        TMP_FontAsset fontAsset = label.font;

        // Compute scale of the target point size relative to the sampling point size of the font asset.
        float pointSizeScale = fontSize / (fontAsset.faceInfo.pointSize * fontAsset.faceInfo.scale);
        float emScale = fontSize * 0.01f;

        float styleSpacingAdjustment = (style & FontStyles.Bold) == FontStyles.Bold ? fontAsset.boldSpacing : 0;
        float normalSpacingAdjustment = fontAsset.normalSpacingOffset;

        float width = 0;

        foreach (var unicode in text)
        {
            // Make sure the given unicode exists in the font asset.
            if (fontAsset.characterLookupTable.TryGetValue(unicode, out var character))
                width += character.glyph.metrics.horizontalAdvance * pointSizeScale + (styleSpacingAdjustment + normalSpacingAdjustment) * emScale;
        }

        return width;
    }

    // Yes I know this is very lazy.
    protected float TextHeight(string text)
    {
        if (!label)
            return 0;
        
        label.SetText(text);
        
        return label.preferredHeight;
    }
}
