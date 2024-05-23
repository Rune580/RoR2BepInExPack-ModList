using JetBrains.Annotations;
using Markdig.Parsers;
using Markdig.Syntax;

namespace RoR2BepInExPack.ModListSystem.Markdown.UnityExt.Syntax;

public class DetailsBlock : Block
{
    [CanBeNull]
    public string SummaryMarkdown { get; set; }
    
    public string ContentMarkdown { get; set; }
    
    public DetailsBlock([CanBeNull] BlockParser parser) : base(parser)
    {
        ContentMarkdown = "";
    }
    
    public void AddSummaryLine(string line)
    {
        if (!string.IsNullOrEmpty(SummaryMarkdown))
            SummaryMarkdown += "\n";

        SummaryMarkdown += line;
    }

    public void AddContentLine(string line)
    {
        if (!string.IsNullOrEmpty(ContentMarkdown))
            ContentMarkdown += "\n";

        ContentMarkdown += line;
    }
}
