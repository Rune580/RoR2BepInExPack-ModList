using JetBrains.Annotations;
using Markdig.Parsers;
using Markdig.Syntax;

namespace RoR2BepInExPack.ModListSystem.Markdown.UnityExt.Syntax;

public class DetailsSummaryBlock : ContainerBlock
{
    public bool HasEnd { get; set; }
    
    public DetailsSummaryBlock([CanBeNull] BlockParser parser) : base(parser)
    {
        
    }
}
