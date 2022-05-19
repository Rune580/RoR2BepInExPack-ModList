using System;

namespace RoR2BepInExPack.ModListSystem;

[Serializable]
public struct HyperLink
{
    public string displayName;
    public string link;

    public HyperLink(string displayName, string link)
    {
        this.displayName = displayName;
        this.link = link;
    }
}
