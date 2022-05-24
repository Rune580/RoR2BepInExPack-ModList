using System;
using RoR2.UI;
using UnityEngine.UI;
using static RoR2BepInExPack.ModListSystem.Components.ModList.ModListSortDropdown;

namespace RoR2BepInExPack.ModListSystem.Components.ModList;

public class ModListSortChoice : BaseHGButton
{
    public ModListSortDropdown controller;
    public LanguageTextMeshController label;
    public Image icon;

    public ModListSort ModListSort { get; private set; }

    public bool IsCurrentChoice
    {
        get => controller.CurrentSort == ModListSort;
    }

    public override void Awake()
    {
        base.Awake();

        if (!controller)
            return;
        
        onClick.AddListener(HandleClick);
    }

    private void HandleClick()
    {
        controller.CurrentSort = ModListSort;
    }

    public override void OnEnable()
    {
        base.OnEnable();

        Refresh();
    }

    internal void Refresh()
    {
        if (!controller)
            return;

        interactable = !IsCurrentChoice;
    }

    internal void BindData(ModListSort modListSort)
    {
        if (!controller)
            return;
        
        ModListSort = modListSort;

        label.token = ModListSort.SortToken;
        icon.sprite = ModListSort.DirectionSprite;

        Refresh();
    }
}
