using Markdig.Syntax.Inlines;
using RoR2BepInExPack.ModListSystem.Components.Markdown.BlockObjects;
using RoR2BepInExPack.ModListSystem.Markdown;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoR2BepInExPack.ModListSystem.Components.Markdown.InlineObjects;

public abstract class BaseMarkdownInlineObject : UIBehaviour
{
    private RectTransform _rectTransform;

    protected RectTransform RectTransform
    {
        get
        {
            if (!_rectTransform)
                _rectTransform = GetComponent<RectTransform>();
            
            return _rectTransform;
        }
    }

    protected float AnchoredYPos
    {
        get => RectTransform.anchoredPosition.y;
        set
        {
            var anchoredPosition = RectTransform.anchoredPosition;
            anchoredPosition.y = value;
            RectTransform.anchoredPosition = anchoredPosition;
        }
    }
    
    protected float Width
    {
        get => RectTransform.sizeDelta.x;
        set
        {
            var sizeDelta = RectTransform.sizeDelta;
            sizeDelta.x = value;
            RectTransform.sizeDelta = sizeDelta;
        }
    }
    
    protected float Height
    {
        get => RectTransform.sizeDelta.y;
        set
        {
            var sizeDelta = RectTransform.sizeDelta;
            sizeDelta.y = value;
            RectTransform.sizeDelta = sizeDelta;
        }
    }
    
    internal BaseMarkdownBlockObject ParentBlock { get; set; }
    
    internal BaseMarkdownInlineObject NextSibling { get; set; }
    
    private Inline _inline;
    private RenderContext _renderCtx;
    private RenderContext _refRenderCtx;
    private InlineContext _inlineCtx;
    private InlineContext _refInlineCtx;

    public void PreParse(Inline inline, RenderContext renderCtx, InlineContext inlineCtx)
    {
        _inline = inline;
        _renderCtx = new RenderContext(renderCtx);
        _refRenderCtx = renderCtx;
        _inlineCtx = new InlineContext(inlineCtx);
        _refInlineCtx = inlineCtx;
    }
    
    public abstract void Parse(Inline inline, RenderContext renderCtx, InlineContext inlineCtx);

    protected void Rebuild()
    {
        if (_inline is null || _renderCtx is null || _inlineCtx is null)
            return;

        // var oldHeight = ParentBlock.Height;
        var currentYPos = _renderCtx.YPos;

        if (NextSibling)
        {
            Parse(_inline, _renderCtx, _inlineCtx);
            NextSibling.PreParse(NextSibling._inline, _renderCtx, _inlineCtx);
            
            NextSibling._inlineCtx.XPos = _inlineCtx.XPos;
            NextSibling._inlineCtx.YPos = _inlineCtx.YPos;
            NextSibling._inlineCtx.LineHeight = _inlineCtx.LineHeight;
            NextSibling._renderCtx = _renderCtx;
            
            NextSibling.Rebuild();
            
            var newHeight = _renderCtx.YPos - currentYPos;
            ParentBlock.Height = newHeight;
        }
        else
        {
            _refInlineCtx.XPos = _inlineCtx.XPos;
            _refInlineCtx.YPos = _inlineCtx.YPos;
            _refInlineCtx.LineHeight = _inlineCtx.LineHeight;
            
            Parse(_inline, _refRenderCtx, _refInlineCtx);
            
            var newHeight = _refRenderCtx.YPos - currentYPos;
            ParentBlock.Height = newHeight;
        }
    }

    protected void ReloadParentBlock() => ParentBlock.Reload();
}
