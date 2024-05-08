using System.Collections.Generic;
using System.Linq;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace RoR2BepInExPack.ModListSystem.Markdown;

public class InlineContext
{
    public float FontSize;

    public float XPos;
    public float YPos;
    public bool LastItem;

    public float LineHeight;

    public string Styling
    {
        get
        {
            var styling = "{0}";
            
            foreach (var tag in _tags)
            {
                styling = styling.Insert(0, $"<{tag}>");
                var endTag = tag.Split('=').First();

                styling += $"</{endTag}>";
            }

            return styling;
        }
    }

    private readonly List<string> _tags = [];

    public void AddStyleTags(params string[] tags)
    {
        foreach (var tag in tags)
        {
            var tagToAdd = tag;
            
            if (tagToAdd.StartsWith("<") && tagToAdd.EndsWith(">"))
                tagToAdd = tagToAdd.Substring(1, tagToAdd.Length - 2);

            if (_tags.Contains(tagToAdd))
                continue;
        
            _tags.Add(tagToAdd);
        }
    }

    public void RemoveStyleTags(params string[] tags)
    {
        foreach (var tag in tags)
        {
            var tagsToRemove = tag;
            
            if (tagsToRemove.StartsWith("<") && tagsToRemove.EndsWith(">"))
                tagsToRemove = tagsToRemove.Substring(1, tagsToRemove.Length - 2);

            _tags.Remove(tagsToRemove);
        }
    }

    public bool HasTag(string tag)
    {
        if (tag.StartsWith("<") && tag.EndsWith(">"))
            tag = tag.Substring(1, tag.Length - 2);

        return _tags.Contains(tag);
    }

    public void ClearTags() => _tags.Clear();
}
