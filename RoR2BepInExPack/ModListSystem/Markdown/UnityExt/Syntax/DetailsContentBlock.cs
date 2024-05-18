using JetBrains.Annotations;
using Markdig.Parsers;
using Markdig.Syntax;

namespace RoR2BepInExPack.ModListSystem.Markdown.UnityExt.Syntax;

public class DetailsContentBlock : ContainerBlock
{
    public DetailsContentBlock([CanBeNull] BlockParser parser) : base(parser)
    {
        
    }
}
