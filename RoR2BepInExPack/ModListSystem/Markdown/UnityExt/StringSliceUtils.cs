using System;
using Markdig.Helpers;

namespace RoR2BepInExPack.ModListSystem.Markdown.UnityExt;

internal static class StringSliceUtils
{
    public static bool Match(this StringSlice slice, string text) => slice.Match(text, slice.Start);

    public static void SkipCount(this ref StringSlice slice, int count)
    {
        for (int i = 0; i < count; i++)
            slice.SkipChar();
    }

    /// <summary>
    /// Helper method to force the slice to the next line
    /// </summary>
    /// <param name="slice"></param>
    public static bool NextLine(this ref StringSlice slice)
    {
        var sourcePos = slice.End + 1;

        slice.Start = sourcePos;
        slice.End = slice.Text.Length;

        NewLine newLine;

        var newLinePos = slice.AsSpan().IndexOfAny('\r', '\n');
        if (newLinePos < 0)
            return slice.Start != slice.End;

        var secondCharPos = newLinePos + 1;

        if (newLinePos < slice.End && slice[newLinePos] == '\r')
        {
            if (secondCharPos < slice.End && slice[secondCharPos] == '\n')
            {
                newLine = NewLine.CarriageReturnLineFeed;
                secondCharPos++;
            }
            else
            {
                newLine = NewLine.CarriageReturn;
            }
        }
        else
        {
            newLine = NewLine.LineFeed;
        }

        slice.NewLine = newLine;
        slice.End = secondCharPos;

        return true;
    }
}
