using RoR2BepInExPack.ModListSystem.Markdown.Images;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2BepInExPack.ModListSystem.Components.Markdown;

[RequireComponent(typeof(RawImage))]
[ExecuteAlways]
public class AnimatedImageController : MonoBehaviour
{
    public RawImage targetImage;
    
    private AnimatedImage _animatedImage;
    private float _timer;
    private float _frameDuration;

    internal void SetAnimatedImage(AnimatedImage animatedImage)
    {
        if (animatedImage is null || !targetImage)
            return;
        
        _animatedImage?.Dispose();
        _animatedImage = animatedImage;

        _timer = 0f;
        _frameDuration = animatedImage.FrameDurationMs;

        targetImage.texture = _animatedImage.Texture;
        
        enabled = true;
        targetImage.enabled = true;
    }

    private void OnEnable()
    {
        if (_animatedImage is null)
        {
            if (targetImage)
                targetImage.enabled = false;
            
            enabled = false;
            return;
        }
        
        _animatedImage.SetFrame(0);
    }

    private void Update()
    {
        if (!isActiveAndEnabled)
            return;
        
        _timer += Time.unscaledDeltaTime;
        if (_timer < _frameDuration)
            return;
        
        _animatedImage.NextFrame();

        _timer -= _frameDuration;
        _frameDuration = _animatedImage.FrameDurationMs;
    }
}
