using System;
using System.Collections;
using RoR2BepInExPack.ModListSystem.Components.Markdown.BlockObjects;
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
    private BaseMarkdownBlockObject[] _contentBlockObjects = [];

    private IEnumerator _parseCoroutine;

    public override void Awake()
    {
        base.Awake();

        if (!content)
            return;
        
        if (!isActiveAndEnabled)
            return;

        if (_parseCoroutine is not null)
        {
            StopCoroutine(_parseCoroutine);
            _parseCoroutine = null;
        }
        
        ClearContent();
        
        _parseCoroutine = markdownBlockParser.Parse(markdownText, content, OnBlockObjectCreated);
        StartCoroutine(_parseCoroutine);
    }

    private void OnBlockObjectCreated(BaseMarkdownBlockObject blockObject)
    {
        var index = _contentBlockObjects.Length;
        Array.Resize(ref _contentBlockObjects, index + 1);

        _contentBlockObjects[index] = blockObject;
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

        foreach (var contentObject in _contentBlockObjects)
            DestroyImmediate(contentObject.gameObject);

        _contentBlockObjects = [];

        var contentSize = content.sizeDelta;
        contentSize.y = 0;
        content.sizeDelta = contentSize;

        content.anchoredPosition = Vector2.zero;
    }
}
