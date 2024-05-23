using System;
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax;
using RoR2BepInExPack.ModListSystem.Markdown.UnityExt.Syntax;

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
            var line = processor.Line;
            
            var text = line.Text.Substring(line.Start, line.Length);
            text = text.Replace("<<<NEWLINE>>>", "\n")
                .Replace("<<<LINEFEED>>>", "\r");

            var detailsLine = new StringSlice(text)
            {
                NewLine = line.NewLine
            };

            var lastEnd = 0;
            var lineIndex = 0;
            var detailsBlock = (DetailsBlock)processor.NewBlocks.Peek();
            
            foreach (var textLine in text.Split('\n', '\r'))
            {
                detailsLine.Start = detailsLine.Text.IndexOf(textLine, lastEnd, StringComparison.InvariantCulture);
                detailsLine.End = detailsLine.Start + textLine.Length - 1;
                
                processor.Line = detailsLine;
                result = MatchEnd(processor, detailsBlock, 0, lineIndex);

                lastEnd = detailsLine.End + 1;
                lineIndex++;
            }

            processor.Line = line;
        }

        return result;
    }

    public override BlockState TryContinue(BlockProcessor processor, Block block)
    {
        if (block is not DetailsBlock detailsBlock)
            return BlockState.None;

        return MatchEnd(processor, detailsBlock, processor.Column, processor.LineIndex);
    }

    public override bool Close(BlockProcessor processor, Block block)
    {
        if (block is not DetailsBlock detailsBlock)
            return false;

        if (string.IsNullOrEmpty(detailsBlock.SummaryMarkdown))
            detailsBlock.SummaryMarkdown = "Details";
        
        return true;
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
        
        line.SkipCount("details>".Length);
        
        var detailsBlock = new DetailsBlock(this)
        {
            Column = processor.ColumnBeforeIndent,
            Span = new SourceSpan(startPos, startPos + line.End)
        };

        if (processor.TrackTrivia)
        {
            detailsBlock.LinesBefore = processor.UseLinesBefore();
            detailsBlock.NewLine = processor.Line.NewLine;
        }
        
        var state = new DetailsBlockState();
        detailsBlock.SetData(typeof(DetailsBlockState), state);
        
        processor.NewBlocks.Push(detailsBlock);
        
        return BlockState.Continue;
    }

    private BlockState MatchEnd(BlockProcessor processor, DetailsBlock detailsBlock, int column, int lineIndex)
    {
        var data = detailsBlock.GetData(typeof(DetailsBlockState));
        if (data is not DetailsBlockState state)
            return BlockState.None;

        var result = BlockState.Continue;
        
        // processor.GoToColumn(processor.ColumnBeforeIndent);
        
        var line = processor.Line;
        var startPos = line.Start;

        if (MatchSummary(processor, detailsBlock, state, out var endSummaryPos))
        {
            // Return early if we are still processing the summary block
            if (endSummaryPos < 0)
                return result;

            startPos = endSummaryPos;
        }
        
        var index = line.IndexOf("</details>");

        line.Start = startPos;

        if (index >= 0)
        {
            line.End = index - 1;
            detailsBlock.AddContentLine(line.ToString());

            detailsBlock.UpdateSpanEnd(index + "</details>".Length);
            result = BlockState.BreakDiscard;
        }
        else
        {
            if (line.Start < line.End && !string.IsNullOrEmpty(line.ToString()))
                detailsBlock.AddContentLine(line.ToString());
        }

        detailsBlock.Span.End = line.End;
        detailsBlock.NewLine = processor.Line.NewLine;

        return result;
    }

    private bool MatchSummary(BlockProcessor processor, DetailsBlock detailsBlock, DetailsBlockState state, out int endPos)
    {
        endPos = -1;
        
        if (!string.IsNullOrEmpty(detailsBlock.SummaryMarkdown) && state.SummaryHasEnd)
            return false;
        
        var line = processor.Line;
        var openPos = line.IndexOf("<summary>");

        if (string.IsNullOrEmpty(detailsBlock.SummaryMarkdown))
        {
            if (openPos < 0)
                return false;
            
            var startPos = openPos + "<summary>".Length;
            
            line.Start = startPos;

            detailsBlock.SummaryMarkdown = "";
        }

        var closePos = line.IndexOf("</summary>");
        
        if (closePos >= 0)
        {
            line.End = closePos - 1;
            
            detailsBlock.AddSummaryLine(line.ToString());
            
            endPos = closePos + "</summary>".Length;

            state.SummaryHasEnd = true;
            detailsBlock.SetData(typeof(DetailsBlockState), state);
            
            return true;
        }
        
        detailsBlock.AddSummaryLine(line.ToString());
        
        detailsBlock.SetData(typeof(DetailsBlockState), state);
        
        return false;
    }
}
