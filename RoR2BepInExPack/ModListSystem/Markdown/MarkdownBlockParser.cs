using System;
using System.Collections;
using System.Collections.Generic;
using Markdig;
using Markdig.Syntax;
using RoR2BepInExPack.ModListSystem.Components.Markdown.BlockObjects;
using RoR2BepInExPack.ModListSystem.Markdown.UnityExt;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace RoR2BepInExPack.ModListSystem.Markdown;

[CreateAssetMenu]
public class MarkdownBlockParser : ScriptableObject
{
    [Header("Render Settings")]
    public float fontSize = 16f;
    
    [Header("Parsers")]
    public MarkdownInlineParser inlineParser;
    public BlockConverter[] converters;

    private readonly Dictionary<BlockType, GameObject> _converterLut = new();
    
    public IEnumerator Parse(string markdown, RectTransform target, Action<BaseMarkdownBlockObject> blockObjectCreatedCallback = null)
    {
        var blockObjects = new List<BaseMarkdownBlockObject>();

        var pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .DisableHtml()
            .UseEmojiAndSmiley(false)
            .Use<UnityExtensions>()
            .Build();
        
        var doc = Markdig.Markdown.Parse(MarkdownPreProcessor.PreProcess(markdown), pipeline);
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(target);
        
        var renderCtx = new RenderContext
        {
            BlockParser = this,
            InlineParser = inlineParser,
            ViewportRect = new Rect(target.rect.position, target.rect.size - new Vector2(20f, 20f)),
            FontSize = fontSize,
            YPos = 0f
        };

        BaseMarkdownBlockObject lastBlock = null;

        foreach (var block in doc)
        {
            var instance = Parse(block, target, renderCtx);
            if (!instance)
                continue;

            if (lastBlock)
                lastBlock.NextSibling = instance;
            
            lastBlock = instance;
            
            blockObjectCreatedCallback?.Invoke(instance);
            yield return null;
        }

        var contentSize = target.sizeDelta;
        contentSize.y = renderCtx.YPos;
        target.sizeDelta = contentSize;
    }
    
    public BaseMarkdownBlockObject Parse(Block block, RectTransform target, RenderContext renderCtx)
    {
        if (!Enum.TryParse<BlockType>(block.GetType().Name, out var blockType))
        {
            Debug.LogWarning($"Could not parse markdown block {block.GetType()}!");
            return null;
        }

        var prefab = GetPrefab(blockType);
        if (!prefab)
            return null;
            
        var instance = Instantiate(prefab, target);
        
        var blockObject = instance.GetComponent<BaseMarkdownBlockObject>();
        if (!blockObject)
            return null;
        
        blockObject.PreParse(block, renderCtx);
        blockObject.Parse(block, renderCtx);

        return blockObject;
    }

    private GameObject GetPrefab(BlockType blockType)
    {
        if (_converterLut.Count == 0)
        {
            foreach (var converter in converters)
                _converterLut[converter.blockType] = converter.prefab;
        }

        if (!_converterLut.TryGetValue(blockType, out var prefab))
        {
            Debug.LogWarning($"No BlockConverter found for {blockType}!");
            return null;
        }

        return prefab;
    }
    
    [Serializable]
    public struct BlockConverter
    {
        public BlockType blockType;
        public GameObject prefab;
    }
    
    public enum BlockType
    {
        BlankLineBlock,
        AlertBlock,
        HeadingLinkReferenceDefinition,
        CustomContainer,
        DefinitionItem,
        DefinitionList,
        DefinitionTerm,
        Figure,
        FigureCaption,
        FooterBlock,
        Footnote,
        FootnoteGroup,
        FootnoteLinkReferenceDefinition,
        MathBlock,
        Table,
        TableCell,
        TableRow,
        YamlFrontMatterBlock,
        Abbreviation,
        FencedCodeBlock,
        CodeBlock,
        LinkReferenceDefinitionGroup,
        ListBlock,
        ListItemBlock,
        MarkdownDocument,
        QuoteBlock,
        ContainerBlock,
        EmptyBlock,
        HeadingBlock,
        HtmlBlock,
        LinkReferenceDefinition,
        ParagraphBlock,
        ThematicBreakBlock,
        LeafBlock,
        DetailsBlock,
    }
}
