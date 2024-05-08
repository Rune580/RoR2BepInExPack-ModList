using System;
using RoR2BepInExPack.ModListSystem.Markdown.Images;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoR2BepInExPack.ModListSystem.Components.Markdown;

[RequireComponent(typeof(RawImage))]
[ExecuteAlways]
public class VectorImageController : UIBehaviour
{
    public RawImage targetImage;
    
    [Range(1, 10)]
    [Tooltip("Increase the Rasterization quality of the vector graphic")]
    public int vectorQualityScale = 1;

    private VectorImage _vectorImage;

    internal void SetVectorImage(VectorImage vectorImage)
    {
        if (vectorImage is null || !targetImage)
            return;

        vectorImage.QualityScale = vectorQualityScale;

        if (_vectorImage is not null)
        {
            _vectorImage.OnTextureResized -= ResetTargetImage;
            _vectorImage.Dispose();
        }
        
        _vectorImage = vectorImage;

        targetImage.texture = _vectorImage.Texture;

        _vectorImage.OnTextureResized += ResetTargetImage;

        enabled = true;
        targetImage.enabled = true;
    }

    private void ResetTargetImage()
    {
        if (_vectorImage is null || !targetImage)
            return;
        
        targetImage.texture = _vectorImage.Texture;
    }

    private void OnValidate()
    {
        if (_vectorImage is not null)
            _vectorImage.QualityScale = vectorQualityScale;
    }

    public override void OnEnable()
    {
        if (_vectorImage is null)
        {
            if (targetImage)
                targetImage.enabled = false;

            enabled = false;
            return;
        }
        
        base.OnEnable();
    }
}
