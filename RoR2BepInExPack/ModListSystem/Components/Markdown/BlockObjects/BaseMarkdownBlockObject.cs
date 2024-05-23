using Markdig.Syntax;
using RoR2BepInExPack.ModListSystem.Markdown;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoR2BepInExPack.ModListSystem.Components.Markdown.BlockObjects;

[RequireComponent(typeof(RectTransform))]
[ExecuteAlways]
public abstract class BaseMarkdownBlockObject : UIBehaviour, ILayoutElement
{
    private RectTransform _rectTransform;

    public RectTransform RectTransform
    {
        get
        {
            if (!_rectTransform)
                _rectTransform = GetComponent<RectTransform>();
            
            return _rectTransform;
        }
    }

    protected Vector2 AnchoredPosition
    {
        get => RectTransform.anchoredPosition;
        set => RectTransform.anchoredPosition = value;
    }

    protected float AnchoredYPosition
    {
        get => AnchoredPosition.y;
        set
        {
            var anchoredPos = AnchoredPosition;
            var oldYPos = anchoredPos.y;
            anchoredPos.y = value;
            AnchoredPosition = anchoredPos;

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (oldYPos != value)
            {
                if (!NextSibling)
                    return;
                
                NextSibling.OnPrevSiblingAnchoredYPositionChanged(value - oldYPos);
            }
        }
    }

    private float _lastHeight;

    protected internal float Height
    {
        get => RectTransform.sizeDelta.y;
        set
        {
            var sizeDelta = RectTransform.sizeDelta;
            var oldHeight = sizeDelta.y;
            sizeDelta.y = value;
            RectTransform.sizeDelta = sizeDelta;

            _lastHeight = value;

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (oldHeight != value)
            {
                var heightDelta = value - oldHeight;
                OnHeightChanged(heightDelta);
                
                if (!NextSibling)
                    return;
                
                NextSibling.OnPrevSiblingHeightChanged(heightDelta);
            }
        }
    }
    
    internal BaseMarkdownBlockObject NextSibling { get; set; }

    private Block _block;
    private RenderContext _renderCtx;

    public void PreParse(Block block, RenderContext renderCtx)
    {
        _block = block;
        _renderCtx = new RenderContext(renderCtx);
    }

    public abstract void Parse(Block block, RenderContext renderCtx);

    public void Reload()
    {
        if (_block is null || _renderCtx is null)
            return;
        
        Parse(_block, _renderCtx);
    }

    protected virtual void OnHeightChanged(float heightDelta)
    {
        var parent = RectTransform.parent as RectTransform;
        if (!parent)
            return;

        var sizeDelta = parent.sizeDelta;
        sizeDelta.y += heightDelta;
        parent.sizeDelta = sizeDelta;
    }

    public virtual void OnPrevSiblingHeightChanged(float heightDelta)
    {
        AnchoredYPosition -= heightDelta;
    }

    public virtual void OnPrevSiblingAnchoredYPositionChanged(float yDelta)
    {
        AnchoredYPosition += yDelta;
    }

    // public override void OnRectTransformDimensionsChange()
    // {
    //     // ReSharper disable once CompareOfFloatsByEqualityOperator
    //     if (Height == _lastHeight)
    //         return;
    //
    //     var delta = Height - _lastHeight;
    //     _lastHeight = Height;
    //     
    //     OnHeightChanged(delta);
    // }

    #region LayoutElement

    public void CalculateLayoutInputHorizontal() { }

    public void CalculateLayoutInputVertical() { }

    public virtual float minWidth { get; protected set; }
    public virtual float preferredWidth { get; protected set; }
    public virtual float flexibleWidth { get; protected set; }
    public virtual float minHeight { get; protected set; }
    public virtual float preferredHeight { get; protected set; }
    public virtual float flexibleHeight { get; protected set; }
    public int layoutPriority => 0;

    #endregion
}
