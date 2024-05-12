using System.Text.RegularExpressions;

namespace RoR2BepInExPack.ModListSystem.Components;

internal static class MarkdownPreProcessor
{
    private static readonly Regex HtmlTagStrongRegex = new(@"\<strong\>(.*)\</strong\>");
    private static readonly Regex HtmlTagLineBreakRegex = new(@"(\<br\>)");
    
    public static string PreProcess(string markdown)
    {
        var result = HtmlTagStrongRegex.Replace(markdown, "*$1*");
        result = HtmlTagLineBreakRegex.Replace(result, "\r");

        return result;
    }
}
