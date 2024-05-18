using Markdig;
using Markdig.Renderers;
using RoR2BepInExPack.ModListSystem.Markdown.UnityExt.Parsers;

namespace RoR2BepInExPack.ModListSystem.Markdown.UnityExt;

internal class UnityExtensions : IMarkdownExtension
{
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        pipeline.BlockParsers
            .AddIfNotAlready<DetailsBlockParser>();
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer) { }
}
