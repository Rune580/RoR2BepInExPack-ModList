using RoR2BepInExPack.ModListSystem.Markdown;
using UnityEngine;
using UnityEngine.EventSystems;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace RoR2BepInExPack.ModListSystem.Components.Markdown;

public class MarkdownConverter : UIBehaviour
{
    public MarkdownBlockParser markdownBlockParser;
    public RectTransform content;
    [Multiline]
    public string markdownText;

    [SerializeField, HideInInspector]
    private GameObject[] _contentObjects = [];

    public override void Awake()
    {
        base.Awake();

        if (!content)
            return;
        
        if (!isActiveAndEnabled)
            return;

        ClearContent();
        
        _contentObjects = markdownBlockParser.Parse(markdownText, content);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        
        ClearContent();
    }

    public void ClearContent()
    {
        if (!content)
            return;

        foreach (var contentObject in _contentObjects)
            DestroyImmediate(contentObject);

        _contentObjects = [];

        var contentSize = content.sizeDelta;
        contentSize.y = 0;
        content.sizeDelta = contentSize;

        content.anchoredPosition = Vector2.zero;
    }
}
