using System.Collections.Generic;
using System.Linq;

namespace RoR2BepInExPack.ModListSystem.Markdown;

public class TextStyling
{
    private readonly List<string> _styleTags = [];
    
    public void AddStyleTags(params string[] tags)
    {
        foreach (var tag in tags)
        {
            var tagToAdd = tag;
            
            if (tagToAdd.StartsWith("<") && tagToAdd.EndsWith(">"))
                tagToAdd = tagToAdd.Substring(1, tagToAdd.Length - 2);

            if (_styleTags.Contains(tagToAdd))
                continue;
        
            _styleTags.Add(tagToAdd);
        }
    }

    public void RemoveStyleTags(params string[] tags)
    {
        foreach (var tag in tags)
        {
            var tagsToRemove = tag;
            
            if (tagsToRemove.StartsWith("<") && tagsToRemove.EndsWith(">"))
                tagsToRemove = tagsToRemove.Substring(1, tagsToRemove.Length - 2);

            _styleTags.Remove(tagsToRemove);
        }
    }
    
    public bool HasTag(string tag)
    {
        if (tag.StartsWith("<") && tag.EndsWith(">"))
            tag = tag.Substring(1, tag.Length - 2);

        return _styleTags.Contains(tag);
    }
    
    public void ClearTags() => _styleTags.Clear();

    public string StyledTemplate
    {
        get
        {
            var styling = "{0}";
            
            foreach (var tag in _styleTags)
            {
                styling = styling.Insert(0, $"<{tag}>");
                var endTag = tag.Split('=').First();

                styling += $"</{endTag}>";
            }

            return styling;
        }
    }
}
