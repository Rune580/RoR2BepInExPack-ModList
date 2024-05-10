using Markdig.Syntax;
using RoR2BepInExPack.ModListSystem.Markdown;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoR2BepInExPack.ModListSystem.Components.Markdown.BlockObjects;

[RequireComponent(typeof(RectTransform))]
public abstract class BaseMarkdownBlockObject : UIBehaviour, ILayoutElement
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

    public abstract void Parse(Block block, RenderContext renderCtx);
    
    public void CalculateLayoutInputHorizontal() { }

    public void CalculateLayoutInputVertical() { }

    public virtual float minWidth { get; protected set; }
    public virtual float preferredWidth { get; protected set; }
    public virtual float flexibleWidth { get; protected set; }
    public virtual float minHeight { get; protected set; }
    public virtual float preferredHeight { get; protected set; }
    public virtual float flexibleHeight { get; protected set; }
    public int layoutPriority => 0;
}
