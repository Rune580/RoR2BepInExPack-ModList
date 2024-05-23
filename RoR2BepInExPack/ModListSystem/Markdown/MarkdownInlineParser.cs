using System;
using System.Collections.Generic;
using Markdig.Syntax.Inlines;
using RoR2BepInExPack.ModListSystem.Components.Markdown.BlockObjects;
using RoR2BepInExPack.ModListSystem.Components.Markdown.InlineObjects;
using UnityEngine;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace RoR2BepInExPack.ModListSystem.Markdown;

[CreateAssetMenu]
public class MarkdownInlineParser : ScriptableObject
{
    public InlineConverter[] converters;
    
    private readonly Dictionary<InlineType, GameObject> _converterLut = new();
    
    public BaseMarkdownInlineObject Parse(Inline inline, RectTransform target, BaseMarkdownBlockObject parent, RenderContext renderCtx, InlineContext inlineCtx)
    {
        if (!Enum.TryParse<InlineType>(inline.GetType().Name, out var inlineType))
        {
            Debug.LogWarning($"Could not parse markdown inline {inline.GetType()}!");
            return null;
        }
        
        var prefab = GetPrefab(inlineType);
        if (!prefab)
            return null;
        
        var instance = Instantiate(prefab, target);

        var inlineObject = instance.GetComponent<BaseMarkdownInlineObject>();
        if (!inlineObject)
            return null;

        inlineObject.ParentBlock = parent;
        inlineObject.PreParse(inline, renderCtx, inlineCtx);
        inlineObject.Parse(inline, renderCtx, inlineCtx);
        
        return inlineObject;
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
