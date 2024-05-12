using Markdig.Helpers;

namespace RoR2BepInExPack.ModListSystem.Markdown.UnityExt;

internal static class StringSliceUtils
{
    public static bool Match(this StringSlice slice, string text) => slice.Match(text, slice.Start);
}
