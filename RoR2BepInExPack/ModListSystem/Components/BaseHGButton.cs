using RoR2;
using RoR2.UI;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2BepInExPack.ModListSystem.Components;

/// <summary>
/// HGButton but without the bloat.
/// </summary>
public class BaseHGButton : MPButton
{
    public Image imageOnHover;
    public Image imageOnInteract;
    
    private SelectionState _previousState = SelectionState.Disabled;
    private bool _isHovering;
    private float _alphaVelocity;
    private float _scaleVelocity;
    private bool _fallback;

    public override void Awake()
    {
        base.Awake();

        _fallback = !RoR2Application.instance || !RoR2Application.instance.gameObject;
    }

    public override void Start()
    {
        base.Start();
        onClick.AddListener(DoClickSound);
    }

    public override void DoStateTransition(SelectionState state, bool instant)
    {
        base.DoStateTransition(state, instant);

        if (state == _previousState)
            return;

        switch (state)
        {
            case SelectionState.Normal:
                _isHovering = false;
                break;
            case SelectionState.Highlighted:
                if (!_fallback)
                    Util.PlaySound("Play_UI_menuHover", RoR2Application.instance.gameObject);
                _isHovering = true;
                break;
            case SelectionState.Pressed:
                _isHovering = true;
                break;
            case SelectionState.Disabled:
                _isHovering = false;
                break;
        }

        _previousState = state;
    }

    private void LateUpdate()
    {
        if (!Application.isPlaying)
            return;

        if (imageOnHover)
        {
            float hoverAlpha = _isHovering ? 1f : 0f;
            float hoverScale = _isHovering ? 1f : 0f;

            Color imageColor = imageOnHover.color;
            Transform imageTransform = imageOnHover.transform;
            
            imageColor.a = Mathf.SmoothDamp(imageColor.a, hoverAlpha, ref _alphaVelocity, 0.03f * colors.fadeDuration, float.PositiveInfinity, Time.unscaledDeltaTime);
            hoverScale = Mathf.SmoothDamp(imageTransform.localScale.x, hoverScale, ref _scaleVelocity, 0.03f, float.PositiveInfinity, Time.unscaledDeltaTime);

            imageOnHover.color = imageColor;
            imageTransform.localScale = new Vector3(hoverScale, hoverScale, hoverScale);
        }

        if (imageOnInteract)
        {
            imageOnInteract.enabled = interactable;
        }
    }

    private void DoClickSound()
    {
        if (!_fallback)
            Util.PlaySound("Play_UI_menuClick", RoR2Application.instance.gameObject);
    }
}
