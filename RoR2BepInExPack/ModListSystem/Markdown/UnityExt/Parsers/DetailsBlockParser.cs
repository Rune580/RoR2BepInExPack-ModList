using System;
using Markdig.Parsers;

namespace RoR2BepInExPack.ModListSystem.Markdown.UnityExt.Parsers;

public class DetailsBlockParser : BlockParser
{
    public DetailsBlockParser()
    {
        OpeningCharacters = ['<'];
    }
    
    public override BlockState TryOpen(BlockProcessor processor)
    {
        var result = MatchStart(processor);

        if (result == BlockState.Continue)
        {
            // Todo
        }

        return result;
    }

    private BlockState MatchStart(BlockProcessor processor)
    {
        if (processor.IsCodeIndent)
            return BlockState.None;

        var line = processor.Line;
        var startPos = processor.Start;
        
        line.SkipChar();

        if (!line.Match("details>"))
            return BlockState.None;

        return BlockState.Continue;
    }

    private BlockState MatchEnd(BlockProcessor processor)
    {
        throw new NotImplementedException();
    }
}
