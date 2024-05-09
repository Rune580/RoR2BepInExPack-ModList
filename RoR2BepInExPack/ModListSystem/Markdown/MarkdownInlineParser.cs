using System;
using System.Collections.Generic;
using Markdig.Syntax.Inlines;
using RoR2BepInExPack.ModListSystem.Components.Markdown.InlineObjects;
using UnityEngine;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace RoR2BepInExPack.ModListSystem.Markdown;

[CreateAssetMenu]
public class MarkdownInlineParser : ScriptableObject
{
    public InlineConverter[] converters;
    
    private readonly Dictionary<InlineType, GameObject> _converterLut = new();
    
    public void Parse(Inline inline, RectTransform target, RenderContext renderCtx, InlineContext inlineCtx)
    {
        if (!Enum.TryParse<InlineType>(inline.GetType().Name, out var inlineType))
        {
            Debug.LogWarning($"Could not parse markdown inline {inline.GetType()}!");
            return;
        }
        
        var prefab = GetPrefab(inlineType);
        if (!prefab)
            return;
        
        var instance = Instantiate(prefab, target);

        var inlineParser = instance.GetComponent<BaseMarkdownInlineObject>();
        if (!inlineParser)
            return;
        
        inlineParser.Parse(inline, renderCtx, inlineCtx);
    }
    
    private GameObject GetPrefab(InlineType inlineType)
    {
        if (_converterLut.Count == 0)
        {
            foreach (var converter in converters)
                _converterLut[converter.inlineType] = converter.prefab;
        }

        if (!_converterLut.TryGetValue(inlineType, out var prefab))
        {
            Debug.LogWarning($"No InlineConverter found for {inlineType}!");
            return null;
        }

        return prefab;
    }
    
    [Serializable]
    public struct InlineConverter
    {
        public InlineType inlineType;
        public GameObject prefab;
    }

    public enum InlineType
    {
        FootnoteLink,
        CustomContainerInline,
        EmojiInline,
        AbbreviationInline,
        JiraLink,
        MathInline,
        SmartyPant,
        PipeTableDelimiterInline,
        TaskList,
        AutolinkInline,
        CodeInline,
        EmphasisDelimiterInline,
        LinkDelimiterInline,
        DelimiterInline,
        EmphasisInline,
        LinkInline,
        ContainerInline,
        HtmlEntityInline,
        HtmlInline,
        LineBreakInline,
        LiteralInline,
        LeafInline,
    }
}
