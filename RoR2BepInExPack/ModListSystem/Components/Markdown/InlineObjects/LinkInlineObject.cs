using System.Collections;
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

    private bool _imageLoaded;
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

        if (linkInline.IsImage && _cachedImage is null && !_imageLoaded)
        {
            StartCoroutine(LoadImage(linkInline));
        }
        else if (linkInline.IsImage && _imageLoaded && _cachedImage is not null)
        {
            HandleImage(renderCtx, inlineCtx);
        }
        else
        {
            image.enabled = false;
        }

        var styling = inlineCtx.Styling;

        foreach (var subInline in linkInline)
        {
            if (subInline is LiteralInline && linkInline.IsImage && _imageLoaded)
                continue;
            
            inlineCtx.LastItem = linkInline.NextSibling is null;
            
            styling.AddStyleTags("u", "color=#00DDFF");
            renderCtx.InlineParser.Parse(subInline, RectTransform, this.ParentBlock, renderCtx, inlineCtx);
            styling.RemoveStyleTags("u", "color=#00DDFF");

            Height = Mathf.Max(Height, inlineCtx.LineHeight);
            inlineCtx.LineHeight = 0;
        }

        inlineCtx.LastItem = false;
    }

    private IEnumerator LoadImage(LinkInline linkInline)
    {
        yield return ImageHelper.GetImage(linkInline.Url, loadedImage =>
        {
            _cachedImage = loadedImage;
            _imageLoaded = true;
            
            Rebuild();
        });
    }

    private void HandleImage(RenderContext renderCtx, InlineContext inlineCtx)
    {
        if (_cachedImage is null)
            return;
        
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

        var maxWidth = renderCtx.ViewportRect.width - (inlineCtx.FontSize / 2f);
        
        if (inlineCtx.XPos > 0 && inlineCtx.XPos + width >= maxWidth)
        {
            inlineCtx.YPos += inlineCtx.LineHeight;
            inlineCtx.XPos = 0;
            inlineCtx.LineHeight = 0;
        }
        else if (inlineCtx.XPos == 0)
        {
            width = Mathf.Min(maxWidth - inlineCtx.XPos, width);
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
