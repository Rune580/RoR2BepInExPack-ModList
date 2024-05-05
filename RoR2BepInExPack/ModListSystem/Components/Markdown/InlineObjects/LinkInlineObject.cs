using System.Text.RegularExpressions;
using Markdig.Syntax.Inlines;
using RoR2BepInExPack.ModListSystem.Markdown.Images;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2BepInExPack.ModListSystem.Components.Markdown.InlineObjects;

public class LinkInlineObject : BaseMarkdownInlineObject
{
    private static readonly Regex URLRegex = new(@"https:\/\/\S*");
    
    public Button linkButton;
    public Image image;

    private string _url;
    
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

        if (linkInline.IsImage)
        {
            var texture = ImageHelper.GetImage(linkInline.Url);

            if (texture)
            {
                image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                image.enabled = true;
            }
        }
        else
        {
            image.enabled = false;
        }

        foreach (var subInline in linkInline)
        {
            if (subInline is LiteralInline && !linkInline.IsImage)
                inlineCtx.AddStyleTags("u", "color=#00DDFF");
            
            renderCtx.InlineParser.Parse(subInline, RectTransform, renderCtx, inlineCtx);
            
            inlineCtx.RemoveStyleTags("u", "color=#00DDFF");
        }

        Height = inlineCtx.YPos;
    }

    public void OpenUrl()
    {
        if (string.IsNullOrEmpty(_url))
            return;
        
        // Todo internal readme links
        
        // Todo prompt user when clicking on external links
        Application.OpenURL(_url);
    }
}
