using System;

namespace RoR2BepInExPack.ModListSystem;

[Serializable]
public struct HyperLink
{
    public string displayNameToken;
    public string link;

    public HyperLink(string displayNameToken, string link)
    {
        this.displayNameToken = displayNameToken;
        this.link = link;
    }
}
