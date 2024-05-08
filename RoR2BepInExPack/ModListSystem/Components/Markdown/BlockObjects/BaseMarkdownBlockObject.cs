using Markdig.Syntax;
using RoR2BepInExPack.ModListSystem.Markdown;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoR2BepInExPack.ModListSystem.Components.Markdown.BlockObjects;

public abstract class BaseMarkdownBlockObject : UIBehaviour
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
}
