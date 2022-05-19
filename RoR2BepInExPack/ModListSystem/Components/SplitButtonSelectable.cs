using RoR2.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoR2BepInExPack.ModListSystem.Components;

[RequireComponent(typeof(MPEventSystemLocator))]
public class SplitButtonSelectable : Selectable
{
    public Selectable settingsButton;
    public Selectable modsButton;

    private MPEventSystem _mpEventSystem;

    public override void Start()
    {
        _mpEventSystem = GetComponent<MPEventSystemLocator>().eventSystem;
        
        base.Start();
    }

    public override Selectable FindSelectableOnLeft()
    {
        if (!_mpEventSystem)
            return settingsButton;
        
        if (_mpEventSystem.currentSelectedGameObject == settingsButton.gameObject)
            return modsButton;

        return settingsButton;
    }

    public override Selectable FindSelectableOnRight()
    {
        if (!_mpEventSystem)
            return modsButton;
        
        if (_mpEventSystem.currentSelectedGameObject == modsButton.gameObject)
            return settingsButton;

        return modsButton;
    }

    public override void OnSelect(BaseEventData eventData)
    {
        settingsButton.Select();
    }

    public override void Select()
    {
        settingsButton.Select();
    }
}
