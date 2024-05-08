using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Markdig.Syntax.Inlines;
using RoR2.UI;
using RoR2BepInExPack.ModListSystem.Markdown;
using TMPro;
using UnityEngine.UI;

namespace RoR2BepInExPack.ModListSystem.Components.Markdown.InlineObjects;

public class LiteralInlineObject : BaseMarkdownInlineObject
{
    private static readonly Regex UnicodeRegex = new(@"\\u([a-zA-Z0-9]*)");
    
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
    }

    protected void SetText(string text, RenderContext renderCtx, InlineContext inlineCtx)
    {
        text = UnicodeRegex.Replace(text, @"\U\\U000$1\E");
        
        AnchoredYPos = -inlineCtx.YPos;

        var style = FontStyles.Normal;

        if (inlineCtx.HasTag("b"))
            style |= FontStyles.Bold;
        if (inlineCtx.HasTag("i"))
            style |= FontStyles.Italic;

        var wrapPerChar = this is CodeInlineObject;

        var lineWidths = TextWidthMultiLine(text, style, inlineCtx, renderCtx.ViewportRect.width, wrapPerChar, out var textPerLine);
        var textWithoutLastLine = string.Join("\n", textPerLine.AsEnumerable().Take(textPerLine.Length - 1));
        var height = TextHeight(textWithoutLastLine);
        
        for (var i = 0; i < textPerLine.Length; i++)
        {
            if (i == 0)
            {
                textPerLine[i] = $"<line-indent={inlineCtx.XPos}px>{inlineCtx.Styling.Replace("{0}", textPerLine[i])}</line-indent>";
                continue;
            }
            
            textPerLine[i] = $"{inlineCtx.Styling.Replace("{0}", textPerLine[i])}";
        }
        
        label.SetText(string.Join("\n", textPerLine));

        inlineCtx.XPos = lineWidths.Last();

        if (lineWidths.Length > 1 && !inlineCtx.LastItem)
        {
            inlineCtx.YPos += height;
        }
        else if (inlineCtx.LastItem)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(RectTransform);
            inlineCtx.YPos += label.preferredHeight;
        }
    }

    private float[] TextWidthMultiLine(string text, FontStyles style, InlineContext inlineCtx, float maxWidth, bool wrapPerChar, out string[] textPerLine)
    {
        int line = 0;
        List<float> lineWidths = [inlineCtx.XPos];
        List<string> textPerLineList = [""];

        var word = "";
        var wordWidth = 0f;
        foreach (var character in text)
        {
            var charWidth = CharacterWidthApproximation(character, style);

            var lineWidth = lineWidths[line];
            var lineText = textPerLineList[line];

            if (lineWidth + wordWidth + charWidth >= maxWidth)
            {
                lineWidth = 0;
                lineWidths.Add(lineWidth);
                
                line++;
                
                textPerLineList.Add("");
                lineText = textPerLineList[line];
            }

            if (wrapPerChar)
            {
                lineText += character;
                lineWidth += charWidth;
            }
            else
            {
                word += character;
                wordWidth += charWidth;
                
                if (character is ' ' or '\n')
                {
                    lineText += word;
                    lineWidth += wordWidth;
                
                    word = "";
                    wordWidth = 0;
                }
            }

            lineWidths[line] = lineWidth;
            textPerLineList[line] = lineText;
        }

        lineWidths[line] += wordWidth;
        textPerLineList[line] += word;

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

    private float CharacterWidthApproximation(char character, FontStyles style)
    {
        if (!label)
            return 0;

        var fontSize = label.fontSize;
        var fontAsset = label.font;
        
        // Compute scale of the target point size relative to the sampling point size of the font asset.
        float pointSizeScale = fontSize / (fontAsset.faceInfo.pointSize * fontAsset.faceInfo.scale);
        float emScale = fontSize * 0.01f;
        
        float styleSpacingAdjustment = (style & FontStyles.Bold) == FontStyles.Bold ? fontAsset.boldSpacing : 0;
        float normalSpacingAdjustment = fontAsset.normalSpacingOffset;

        if (!fontAsset.characterLookupTable.TryGetValue(character, out var fontChar))
            return 0;
        
        return fontChar.glyph.metrics.horizontalAdvance * pointSizeScale + (styleSpacingAdjustment + normalSpacingAdjustment) * emScale;
    }

    // Yes I know this is very lazy.
    protected float TextHeight(string text)
    {
        if (!label)
            return 0;

        if (string.IsNullOrEmpty(text))
        {
            label.SetText(" ");
        }
        else
        {
            label.SetText(text);
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(label.rectTransform);
        
        return label.preferredHeight;
    }
}
