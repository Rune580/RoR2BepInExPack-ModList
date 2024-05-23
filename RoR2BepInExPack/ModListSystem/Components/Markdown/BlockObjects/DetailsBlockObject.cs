using Markdig.Syntax;
using RoR2BepInExPack.ModListSystem.Markdown;
using RoR2BepInExPack.ModListSystem.Markdown.UnityExt.Syntax;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2BepInExPack.ModListSystem.Components.Markdown.BlockObjects;

[ExecuteAlways]
public class DetailsBlockObject : BaseMarkdownBlockObject
{
    public RectTransform summary;
    public RectTransform content;
    public bool expand;

    private float _collapsedHeight;
    private float _contentPadding;
    
    public override void Parse(Block block, RenderContext renderCtx)
    {
        if (!summary || !content)
            return;
        
        if (block is not DetailsBlock detailsBlock)
            return;

        _contentPadding = renderCtx.FontSize / 2f;
        
        AnchoredPosition = new Vector2(renderCtx.XPos, -renderCtx.YPos);

        if (!string.IsNullOrEmpty(detailsBlock.SummaryMarkdown))
            StartCoroutine(renderCtx.BlockParser.Parse(detailsBlock.SummaryMarkdown, summary));

        _collapsedHeight = summary.sizeDelta.y;
        content.anchoredPosition = new Vector2(0, -(summary.sizeDelta.y + renderCtx.FontSize / 2f));
        
        StartCoroutine(renderCtx.BlockParser.Parse(detailsBlock.ContentMarkdown, content));
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        content.gameObject.SetActive(false);

        Height = _collapsedHeight;
        renderCtx.YPos += Height;
    }

    public void ToggleExpand()
    {
        expand = !expand;
        UpdateOnExpand();
    }

    private void UpdateOnExpand()
    {
        content.gameObject.SetActive(expand);
        
        if (expand)
        {
            Height = _collapsedHeight + content.sizeDelta.y + _contentPadding;
        }
        else
        {
            Height = _collapsedHeight;
        }
    }

    private void OnValidate()
    {
        UpdateOnExpand();
    }
}
