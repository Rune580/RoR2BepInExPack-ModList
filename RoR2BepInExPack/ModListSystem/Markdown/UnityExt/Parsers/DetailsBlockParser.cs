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
            result = MatchEnd(processor, (DetailsBlock)processor.NewBlocks.Peek());
        }

        return result;
    }

    public override BlockState TryContinue(BlockProcessor processor, Block block)
    {
        if (block is not DetailsBlock detailsBlock)
            return BlockState.None;

        return MatchEnd(processor, detailsBlock);
    }

    public override bool Close(BlockProcessor processor, Block block)
    {
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

        startPos += "<details>".Length;

        var span = new SourceSpan(startPos, startPos + line.End);
        var content = CreateParagraphBlock(processor, span);
        processor.Open(content);
        content.IsOpen = true;
        
        detailsBlock.Add(content);
        
        processor.NewBlocks.Push(content);

        var state = new DetailsBlockState();
        detailsBlock.SetData(typeof(DetailsBlockState), state);
        
        processor.NewBlocks.Push(detailsBlock);
        return BlockState.Continue;
    }

    private BlockState MatchEnd(BlockProcessor processor, DetailsBlock detailsBlock)
    {
        var data = detailsBlock.GetData(typeof(DetailsBlockState));
        if (data is not DetailsBlockState state)
            return BlockState.None;

        var result = BlockState.Continue;
        
        processor.GoToColumn(processor.ColumnBeforeIndent);
        
        var line = processor.Line;
        var startPos = line.Start;

        if (MatchSummary(processor, detailsBlock, state, out var endSummaryPos))
        {
            // Return early if we are still processing the summary block
            if (endSummaryPos < 0)
                return result;

            startPos = endSummaryPos;
        }

        var content = (ParagraphBlock)detailsBlock[0];

        do
        {
            var index = line.IndexOf("</details>");

            line.Start = startPos;

            if (index >= 0)
            {
                line.End = index - 1;
                content.AppendLine(ref line, processor.Column, processor.LineIndex, startPos, processor.TrackTrivia);
                content.UpdateSpanEnd(index);

                processor.Close(content);
                content.IsOpen = false;

                detailsBlock.UpdateSpanEnd(index + "</details>".Length);
                result = BlockState.BreakDiscard;
            }
            else
            {
                if (line.Start < line.End && !string.IsNullOrEmpty(line.ToString()))
                    content.AppendLine(ref line, processor.Column, processor.LineIndex, startPos,
                        processor.TrackTrivia);
            }
        } while (line.NextLine());

        detailsBlock.Span.End = line.End;
        detailsBlock.NewLine = processor.Line.NewLine;

        return result;
    }

    private bool MatchSummary(BlockProcessor processor, DetailsBlock detailsBlock, DetailsBlockState state, out int endPos)
    {
        endPos = -1;
        
        if (detailsBlock.SummaryContainer is not null && state.SummaryHasEnd)
            return false;
        
        var line = processor.Line;
        var openPos = line.IndexOf("<summary>");

        if (detailsBlock.SummaryContainer is null)
        {
            if (openPos < 0)
                return false;
            
            var startPos = openPos + "<summary>".Length;
            var span = new SourceSpan(startPos, startPos + line.End);
            
            line.Start = startPos;
            
            var summaryBlock = CreateParagraphBlock(processor, span, detailsBlock);
            detailsBlock.SummaryContainer = summaryBlock;
            processor.Open(detailsBlock.SummaryContainer!);
            detailsBlock.SummaryContainer.IsOpen = true;
        }
        
        do
        {
            var summary = (ParagraphBlock)detailsBlock.SummaryContainer;

            var closePos = line.IndexOf("</summary>");
        
            if (closePos >= 0)
            {
                line.End = closePos - 1;
                summary.AppendLine(ref line, processor.Column, processor.LineIndex, line.Start, processor.TrackTrivia);
            
                endPos = closePos + "</summary>".Length;
            
                detailsBlock.SummaryContainer.UpdateSpanEnd(closePos);
                state.SummaryHasEnd = true;
            
                processor.Close(detailsBlock.SummaryContainer);
                detailsBlock.SummaryContainer.IsOpen = false;
                
                break;
            }
            summary.AppendLine(ref line, processor.Column, processor.LineIndex, line.Start, processor.TrackTrivia);
        } while (line.NextLine());
        
        detailsBlock.SetData(typeof(DetailsBlockState), state);

        return true;
    }

    private ParagraphBlock CreateParagraphBlock(BlockProcessor processor, SourceSpan span, DetailsBlock detailsBlock = null)
    {
        var parser = processor.Parsers.FindExact<ParagraphBlockParser>();
        
        var block = new ParagraphBlock(parser)
        {
            ProcessInlines = true,
            Column = processor.Column,
            Span = span,
            Lines = []
        };

        if (detailsBlock != null)
            block.Parent = detailsBlock;

        return block;
    }
}
