using Markdig.Syntax.Inlines;
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
    
    public abstract void Parse(Inline inline, RenderContext renderCtx, InlineContext inlineCtx);
}
