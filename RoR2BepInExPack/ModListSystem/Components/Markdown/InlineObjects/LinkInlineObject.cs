using System;
using System.Text.RegularExpressions;
using Markdig.Syntax.Inlines;
using RoR2BepInExPack.ModListSystem.Markdown;
using RoR2BepInExPack.ModListSystem.Markdown.Images;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2BepInExPack.ModListSystem.Components.Markdown.InlineObjects;

public class LinkInlineObject : BaseMarkdownInlineObject
{
    private static readonly Regex URLRegex = new(@"https:\/\/\S*");
    
    public Button linkButton;
    public Image image;
    public AnimatedImageController animatedImageController;
    public VectorImageController vectorImageController;

    private string _url;
    private BaseImage _cachedImage;
    
    public override void Parse(Inline inline, RenderContext renderCtx, InlineContext inlineCtx)
    {
        if (!linkButton || !image)
            return;
        
        if (inline is not LinkInline linkInline)
            return;

        if (!linkInline.IsImage && !string.IsNullOrEmpty(linkInline.Url) && URLRegex.IsMatch(linkInline.Url))
        {
            _url = linkInline.Url;
            linkButton.enabled = true;
        }
        else
        {
            linkButton.enabled = false;
        }

        var handledImage = false;
        if (linkInline.IsImage)
        {
            try
            {
                handledImage = HandleImage(linkInline, renderCtx, inlineCtx);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        else
        {
            image.enabled = false;
        }

        foreach (var subInline in linkInline)
        {
            if (subInline is LiteralInline && linkInline.IsImage && handledImage)
                continue;
            
            inlineCtx.LastItem = linkInline.NextSibling is null;
            
            inlineCtx.AddStyleTags("u", "color=#00DDFF");
            renderCtx.InlineParser.Parse(subInline, RectTransform, renderCtx, inlineCtx);
            inlineCtx.RemoveStyleTags("u", "color=#00DDFF");

            Height = Mathf.Max(Height, inlineCtx.LineHeight);
            inlineCtx.LineHeight = 0;
        }

        inlineCtx.LastItem = false;
    }

    private bool HandleImage(LinkInline linkInline, RenderContext renderCtx, InlineContext inlineCtx)
    {
        _cachedImage = ImageHelper.GetImage(linkInline.Url);

        if (_cachedImage is null)
            return false;

        var texture = _cachedImage.Texture;

        float width = _cachedImage.Width;
        float height = _cachedImage.Height;

        var aspectRatio = width / height;

        RectTransform imageRt;

        switch (_cachedImage)
        {
            case VectorImage vectorImage:
                image.enabled = false;

                vectorImageController.SetVectorImage(vectorImage);

                imageRt = vectorImageController.targetImage.rectTransform;
                break;
            case AnimatedImage animatedImage:
                image.enabled = false;

                animatedImageController.SetAnimatedImage(animatedImage);

                imageRt = animatedImageController.targetImage.rectTransform;
                break;
            default:
                image.sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
                image.preserveAspect = true;
                image.enabled = true;

                imageRt = image.rectTransform;
                break;
        }

        if (inlineCtx.XPos > 0 && inlineCtx.XPos + width >= renderCtx.ViewportRect.width)
        {
            inlineCtx.YPos += inlineCtx.LineHeight;
            inlineCtx.XPos = 0;
            inlineCtx.LineHeight = 0;
        }
        else if (inlineCtx.XPos == 0)
        {
            width = Mathf.Min(renderCtx.ViewportRect.width - inlineCtx.XPos, width);
            height = width / aspectRatio;

            imageRt.anchoredPosition = new Vector2(inlineCtx.XPos, -inlineCtx.YPos);
            inlineCtx.SetPreferredWidthIfBigger(inlineCtx.XPos + width);
            inlineCtx.XPos += width + 8f;
        }
        else
        {
            imageRt.anchoredPosition = new Vector2(inlineCtx.XPos, -inlineCtx.YPos);
            inlineCtx.SetPreferredWidthIfBigger(inlineCtx.XPos + width);
            inlineCtx.XPos += width + 8f;
        }

        imageRt.sizeDelta = new Vector2(width, height);
        
        Height = height;
        inlineCtx.LineHeight = Mathf.Max(inlineCtx.LineHeight, Height);

        if (inlineCtx.LastItem && inlineCtx.LineHeight != 0)
        {
            inlineCtx.YPos += inlineCtx.LineHeight;
            inlineCtx.LineHeight = 0;
        }

        return true;
    }

    public void OpenUrl()
    {
        if (string.IsNullOrEmpty(_url))
            return;
        
        // Todo internal readme links
        
        // Todo prompt user when clicking on external links
        Application.OpenURL(_url);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        
        _cachedImage?.Dispose();
    }
}
