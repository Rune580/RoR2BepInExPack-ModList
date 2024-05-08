using System;

namespace RoR2BepInExPack.ModListSystem.Markdown.Images;

internal enum ImageType
{
    Unknown,
    Svg,
    Png,
    Jpeg,
    Gif
}

internal static class ImageTypeExtensions
{
    public static string GetExt(this ImageType imageType)
    {
        return imageType switch {
            ImageType.Unknown => "",
            ImageType.Svg => ".svg",
            ImageType.Png => ".png",
            ImageType.Jpeg => ".jpeg",
            ImageType.Gif => ".gif",
            _ => throw new ArgumentOutOfRangeException(nameof(imageType), imageType, null)
        };
    }
}
