using System;
using UnityEngine;

namespace RoR2BepInExPack.ModListSystem.Markdown.Images;

internal abstract class BaseImage : IDisposable
{
    public abstract Texture2D Texture { get; }

    public virtual float Width => Texture.width;
    
    public virtual float Height => Texture.height;

    public virtual void Dispose() { }
}
