using JetBrains.Annotations;
using Markdig.Parsers;
using Markdig.Syntax;

namespace RoR2BepInExPack.ModListSystem.Markdown.UnityExt.Syntax;

public class DetailsBlock : ContainerBlock
{
    [CanBeNull]
    public Block SummaryContainer { get; set; }
    
    public DetailsBlock([CanBeNull] BlockParser parser) : base(parser)
    {
        IsParagraphBlock = true;
        
    }
}
