using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RoR2BepInExPack.ModListSystem.Components.ModList;

public class ModListSortDropdown : BaseHGButton
{
    public UILayerKey overlayTopLayer;
    public Sprite descendingSprite;
    public Sprite ascendingSprite;
    public RectTransform dropdown;
    public RectTransform content;
    public LanguageTextMeshController previewLabel;
    public Image previewIcon;
    public GameObject choicePrefab;
    public readonly ModListSortDropdownEvent OnValueChanged = new();

    private readonly List<ModListSort> _sorts = new();
    private readonly List<ModListSortChoice> _choices = new();
    private ModListSort _currentSort;

    public ModListSort CurrentSort
    {
        get => _currentSort;
        set
        {
            if (_currentSort == value)
                return;
            
            _currentSort = value;
            HideDropdown();
            UpdatePreview();
            
            OnValueChanged.Invoke(_currentSort);
        }
    }

    public override void Awake()
    {
        base.Awake();
        GenSorts();
        InitChoices();
    }

    public override void Start()
    {
        base.Start();
        UpdateDropdownSize();
    }

    public void ShowDropdown()
    {
        if (!dropdown)
            return;
        
        dropdown.gameObject.SetActive(true);
        UpdateDropdownSize();
    }

    public new void Update()
    {
        base.Update();

        if (dropdown)
            UpdateDropdownSize();
    }

    private void UpdateDropdownSize()
    {
        dropdown.drivenByObject = this;
        dropdown.drivenProperties = DrivenTransformProperties.SizeDeltaY;
        
        Vector2 max = dropdown.sizeDelta;
        dropdown.sizeDelta = new Vector2(max.x, LayoutUtility.GetPreferredHeight(content));
    }

    public void ToggleDropdown()
    {
        if (!dropdown)
            return;
        
        if (dropdown.gameObject.activeSelf)
        {
            HideDropdown();
        }
        else
        {
            ShowDropdown();
        }
    }

    public void HideDropdown()
    {
        if (!dropdown)
            return;
        
        dropdown.gameObject.SetActive(false);
    }

    private void UpdatePreview()
    {
        previewLabel.token = _currentSort.SortToken;
        previewIcon.sprite = _currentSort.DirectionSprite;
    }

    private void GenSorts()
    {
        _sorts.Clear();

        foreach (ModListSortingTypes sortingType in Enum.GetValues(typeof(ModListSortingTypes)))
        {
            foreach (SortDirections direction in Enum.GetValues(typeof(SortDirections)))
            {
                Sprite directionSprite = direction switch
                {
                    SortDirections.Ascending => ascendingSprite,
                    SortDirections.Descending => descendingSprite
                };
                
                ModListSort sort = new ModListSort(sortingType, direction, directionSprite);

                if (sort.SortingType == ModListSortingTypes.Alphabetical && sort.Direction == SortDirections.Descending)
                    CurrentSort = sort;
                
                _sorts.Add(sort);
            }
        }
    }

    private void InitChoices()
    {
        if (!content)
            return;

        if (!choicePrefab)
            return;

        if (!Application.isPlaying)
            return;

        if (_choices.Count > 0)
        {
            foreach (var choice in _choices)
                DestroyImmediate(choice.gameObject);
            
            _choices.Clear();
        }
        
        foreach (ModListSort sort in _sorts)
        {
            ModListSortChoice choice = Instantiate(choicePrefab, content).GetComponent<ModListSortChoice>();
            choice.requiredTopLayer = overlayTopLayer;
            choice.controller = this;
            
            choice.BindData(sort);

            _choices.Add(choice);
        }
    }

    public enum ModListSortingTypes
    {
        Alphabetical,
        Author,
        LoadOrder
    }
    
    public enum SortDirections
    {
        Descending,
        Ascending
    }
    
    public struct ModListSort
    {
        public ModListSortingTypes SortingType;
        public SortDirections Direction;
        public Sprite DirectionSprite;

        public string SortToken
        {
            get
            {
                return SortingType switch
                {
                    ModListSortingTypes.Alphabetical => "MOD_LIST_SORT_TYPE_ALPHABETICAL",
                    ModListSortingTypes.LoadOrder => "MOD_LIST_SORT_TYPE_LOAD",
                    ModListSortingTypes.Author => "MOD_LIST_SORT_TYPE_AUTHOR",
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }

        public ModListSort(ModListSortingTypes sortingType, SortDirections direction, Sprite directionSprite)
        {
            SortingType = sortingType;
            Direction = direction;
            DirectionSprite = directionSprite;
        }

        public bool Equals(ModListSort other) => SortingType == other.SortingType && Direction == other.Direction;

        public override bool Equals(object obj) => obj is ModListSort other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)SortingType * 397) ^ (int)Direction;
            }
        }

        public static bool operator ==(ModListSort left, ModListSort right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ModListSort left, ModListSort right)
        {
            return !(left == right);
        }
    }
    
    public class ModListSortDropdownEvent : UnityEvent<ModListSort> { }
}
