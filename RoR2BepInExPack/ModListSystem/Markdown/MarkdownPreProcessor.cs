using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RoR2BepInExPack.ModListSystem.Markdown;

internal static class MarkdownPreProcessor
{
    private static readonly Regex HtmlTagStrongRegex = new(@"\<strong\>(.*)\</strong\>");
    private static readonly Regex HtmlTagLineBreakRegex = new(@"(\<br\>)");
    
    public static string PreProcess(string markdown)
    {
        var result = HtmlTagStrongRegex.Replace(markdown, "*$1*");
        result = HtmlTagLineBreakRegex.Replace(result, "\r");
        
        result = result.Replace("<p>", "")
            .Replace("</p>", "");

        result = ParseDetailBlocks(result);

        return result;
    }

    private static string ParseDetailBlocks(string markdown)
    {
        var startPos = -1;
        int endPos;
        var depth = 0;
        
        var detailsToInject = new List<(int, string)>();

        do
        {
            var pos = markdown.IndexOf("<details>", startPos + "<details>".Length, StringComparison.InvariantCulture);

            if (depth == 0)
                startPos = pos;

            if (pos != -1)
                depth++;
            
            endPos = markdown.IndexOf("</details>", pos + "<details>".Length, StringComparison.InvariantCulture);
            if (endPos != -1)
                depth--;

            if (startPos != -1 && endPos != -1 && depth == 0)
            {
                var contentLength = (endPos + "</details>".Length) - startPos;
                var detailsContent = markdown.Substring(startPos, contentLength);

                detailsContent = detailsContent
                    .Replace("\n", "<<<NEWLINE>>>")
                    .Replace("\r", "<<<LINEFEED>>>");
                
                markdown = markdown.Remove(startPos, contentLength);
                
                detailsToInject.Add((startPos, detailsContent));

                startPos = 0;
                endPos = 0;
            }

        } while (startPos != -1 && endPos != -1);

        foreach (var (pos, content) in detailsToInject)
            markdown = markdown.Insert(pos, content);

        return markdown;
    }
}
