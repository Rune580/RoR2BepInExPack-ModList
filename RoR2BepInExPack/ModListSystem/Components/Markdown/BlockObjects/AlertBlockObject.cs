using Markdig.Extensions.Alerts;
using Markdig.Syntax;

namespace RoR2BepInExPack.ModListSystem.Components.Markdown.BlockObjects;

public class AlertBlockObject : BaseMarkdownBlockObject
{
    public override void Parse(Block block, RenderContext renderCtx)
    {
        if (block is not AlertBlock alertBlock)
            return;
        
        
    }
}
