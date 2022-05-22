using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace RoR2BepInExPack.ModListSystem.Components.ModList;

public class ModListController : MonoBehaviour
{
    public GameObject modCardPrefab;
    public TMP_InputField searchField;
    public ScrollRect targetScrollRect;
    public RectOffset padding;
    public float spacing;
    public int maxPoolSize = 10;
    public float coverageMultiplier = 1.5f;
    public float recyclingThreshold = 0.2f;

    private readonly RecycleArray<ModCard> _cardPool = new();
    private readonly Vector3[] _corners = new Vector3[4];
    private readonly ModList _mods = new();
    private RectTransform _viewport;
    private RectTransform _content;
    private Vector2 _prevAnchoredPos;
    private Bounds _viewBounds;
    private bool _recycling;

    public ModListSort Sorting
    {
        get
        {
            return _mods.Sorting;
        }
        set
        {
            _mods.Sorting = value;
        }
    }

    public void Awake()
    {
        if (!targetScrollRect)
            return;

        if (!modCardPrefab)
            return;

        if (!searchField)
            return;
        
        _viewport = targetScrollRect.viewport;

        if (!_viewport)
            return;

        UpdateViewBounds();

        _content = targetScrollRect.content;

        if (!_content)
            return;
        
        _content.drivenByObject = this;
        _content.drivenProperties = DrivenTransformProperties.AnchorMin | DrivenTransformProperties.AnchorMax |
                                    DrivenTransformProperties.AnchoredPosition | DrivenTransformProperties.Pivot |
                                    DrivenTransformProperties.SizeDelta;
        _content.anchorMin = new Vector2(0, 1);
        _content.anchorMax = new Vector2(1, 1);
        _content.pivot = new Vector2(0.5f, 1);
        
        
        _prevAnchoredPos = _content.anchoredPosition;

        if (ModListMain.guidToModDataInfo.Count > 0)
            UpdateDataSet(ModListMain.guidToModDataInfo.Values.ToArray());
        
        searchField.onValueChanged.AddListener(OnSearch);
        searchField.onSubmit.AddListener(OnSearch);

        targetScrollRect.onValueChanged.AddListener(HandleScroll);
    }

    public void UpdateDataSet(ModDataInfo[] mods)
    {
        if (mods.Length == 0)
            return;
        
        _mods.SetData(mods);

        float cardHeight = modCardPrefab.GetComponent<RectTransform>().rect.size.y;
        float contentHeight = (spacing * (_mods.Length - 1)) + (cardHeight * _mods.Length);

        _content.sizeDelta = new Vector2(_content.sizeDelta.x, contentHeight + padding.bottom + padding.top);
        
        SetupPool();
    }

    private void OnSearch(string text)
    {
        
    }

    private void UpdateViewBounds()
    {
        _viewport.GetWorldCorners(_corners);
        float threshold = recyclingThreshold * (_corners[2].y - _corners[0].y);
        
        _viewBounds.min = new Vector3(_corners[0].x, _corners[0].y - threshold);
        _viewBounds.max = new Vector3(_corners[2].x, _corners[2].y + threshold);
    }

    private void SetupPool()
    {
        _cardPool.Clear();

        float requiredCoverage = coverageMultiplier * _viewport.rect.height;
        int minPoolSize = Mathf.Min(_mods.Length, maxPoolSize);

        float currentCoverage = 0;
        float posY = -padding.top;

        while ((_cardPool.Size < minPoolSize || currentCoverage < requiredCoverage) && _cardPool.Size < _mods.Length)
        {
            ModCard modCard = Instantiate(modCardPrefab, _content).GetComponent<ModCard>();
            modCard.gameObject.name = $"ModCard {_cardPool.Size + 1}";

            RectTransform cardTransform = modCard.rectTransform;
            cardTransform.anchoredPosition = new Vector2(padding.left, posY);
            float cardHeight = cardTransform.rect.height;
            posY = (cardTransform.anchoredPosition.y - cardHeight) - spacing;
            currentCoverage += cardHeight + spacing;

            cardTransform.sizeDelta = new Vector2(-(padding.left + padding.right), cardHeight);

            modCard.BindData(_mods[_cardPool.Size].ModData);

            _cardPool.Add(modCard);
        }
    }

    private void HandleScroll(Vector2 _)
    {
        Vector2 dir = _content.anchoredPosition - _prevAnchoredPos;
        
        RecycleOnScroll(dir);

        _prevAnchoredPos = _content.anchoredPosition;
    }

    private void RecycleOnScroll(Vector2 dir)
    {
        if (_recycling)
            return;
        
        UpdateViewBounds();

        if (dir.y > 0 && GetMaxY(_cardPool.GetBottom()) > _viewBounds.min.y)
        {
            RecycleTopToBottom();
        }
        else if (dir.y < 0 && GetMinY(_cardPool.GetTop()) < _viewBounds.max.y)
        {
            RecycleBottomToTop();
        }
    }

    private void RecycleTopToBottom()
    {
        _recycling = true;
        
        while (GetMinY(_cardPool.GetTop()) > _viewBounds.max.y && _cardPool.BottomDataIndex + 1 < _mods.Length)
        {
            MoveCardBelowOther(_cardPool.GetTop(), _cardPool.GetBottom());

            _cardPool.RecycleTopToBottom().BindData(_mods[_cardPool.BottomDataIndex].ModData);
        }

        _recycling = false;
    }

    private void RecycleBottomToTop()
    {
        _recycling = true;
        
        while (GetMaxY(_cardPool.GetBottom()) < _viewBounds.min.y && _cardPool.BottomDataIndex + 1 > _cardPool.Size)
        {
            MoveCardAboveOther(_cardPool.GetBottom(), _cardPool.GetTop());
            
            _cardPool.RecycleBottomToTop().BindData(_mods[_cardPool.TopDataIndex].ModData);
        }

        _recycling = false;
    }

    private void MoveCardBelowOther(ModCard left, ModCard right)
    {
        RectTransform card = left.rectTransform;
        RectTransform other = right.rectTransform;

        float posY = other.anchoredPosition.y - (other.sizeDelta.y + spacing);

        card.anchoredPosition = new Vector2(card.anchoredPosition.x, posY);
    }

    private void MoveCardAboveOther(ModCard left, ModCard right)
    {
        RectTransform card = left.rectTransform;
        RectTransform other = right.rectTransform;

        float posY = other.anchoredPosition.y + other.sizeDelta.y + spacing;

        card.anchoredPosition = new Vector2(card.anchoredPosition.x, posY);
    }

    private float GetMaxY(ModCard modCard)
    {
        modCard.rectTransform.GetWorldCorners(_corners);
        return _corners[1].y;
    }

    private float GetMinY(ModCard modCard)
    {
        modCard.rectTransform.GetWorldCorners(_corners);
        return _corners[0].y;
    }

    private class RecycleArray<TItem> where TItem : Component
    {
        private readonly List<TItem> _pool;
        private int _topIndex;
        private int _bottomIndex;
        
        internal int Size => _pool.Count;
        internal int TopDataIndex { get; private set; }
        internal int BottomDataIndex { get; private set; }
        
        internal RecycleArray()
        {
            _pool = new List<TItem>();
            _topIndex = 0;
            _bottomIndex = 0;
            TopDataIndex = 0;
            BottomDataIndex = 0;
        }

        internal void Add(TItem item)
        {
            _pool.Add(item);

            _bottomIndex = _pool.Count - 1;
            BottomDataIndex = _pool.Count - 1;
        }

        internal TItem RecycleTopToBottom()
        {
            _bottomIndex = _topIndex;
            _topIndex = (_topIndex + 1) % _pool.Count;

            TopDataIndex++;
            BottomDataIndex++;

            return _pool[_bottomIndex];
        }

        internal TItem RecycleBottomToTop()
        {
            _topIndex = _bottomIndex;
            _bottomIndex = (_bottomIndex - 1 + _pool.Count) % _pool.Count;

            TopDataIndex--;
            BottomDataIndex--;
            
            return _pool[_topIndex];
        }

        internal TItem GetTop()
        {
            return _pool[_topIndex];
        }

        internal TItem GetBottom()
        {
            return _pool[_bottomIndex];
        }

        internal void Clear()
        {
            foreach (var item in _pool)
                DestroyImmediate(item.gameObject);
            
            _pool.Clear();
            
            _topIndex = 0;
            _bottomIndex = 0;
            TopDataIndex = 0;
            BottomDataIndex = 0;
        }
    }
    
    public enum ModListSortingTypes
    {
        Alphabetical,
        LoadOrder
        
    }
    
    public enum SortDirections
    {
        Ascending,
        Descending
    }
    
    public struct ModListSort
    {
        public ModListSortingTypes SortingType = ModListSortingTypes.LoadOrder;
        public SortDirections Direction = SortDirections.Descending;
        
        public ModListSort() { }
    }

    private class ModList
    {
        private ModDataInfo[] _data = Array.Empty<ModDataInfo>();
        private readonly List<ModDataInfo> _filteredData = new();
        private int[] _indexMap;
        private ModListSort _sorting;
        
        private int[] _alphabetical;
        // private int[] _lastUpdated;
        private int[] _loadOrder;

        internal int Length => _data.Length;
        
        internal ModListSort Sorting
        {
            get
            {
                return _sorting;
            }
            set
            {
                _sorting = value;
                UpdateSort();
            }
        }

        internal ModList()
        {
            _sorting = new ModListSort
            {
                Direction = SortDirections.Descending,
                SortingType = ModListSortingTypes.Alphabetical
            };
        }

        internal ModList(ModDataInfo[] data) : this()
        {
            SetData(data);
        }

        internal ModDataInfo this[int i]
        {
            get => _data[_indexMap[i]];
        }

        internal void SetData(ModDataInfo[] data)
        {
            _data = data;
            _filteredData.Clear();

            UpdateIndexMaps();
        }

        internal void SetSearchFilter(string filter)
        {
            
        }

        private void UpdateIndexMaps()
        {
            _indexMap = new int[_data.Length];
            
            _alphabetical = _data
                .Select((info, i) => new KeyValuePair<int, ModDataInfo>(i, info))
                .OrderBy(kvp => kvp.Value.ModData.Name)
                .Select(kvp => kvp.Key).ToArray();

            _loadOrder = _data
                .Select((_, i) => i).ToArray();

            UpdateSort();
        }

        private void UpdateSort()
        {
            switch (_sorting.SortingType)
            {
                case ModListSortingTypes.Alphabetical:
                    Array.Copy(_alphabetical, _indexMap, Length);
                    break;
                case ModListSortingTypes.LoadOrder:
                    Array.Copy(_loadOrder, _indexMap, Length);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (_sorting.Direction == SortDirections.Ascending)
                _indexMap = _indexMap.Reverse().ToArray();
        }

        private static bool SearchMatch(string text, string filter)
        {
            text = text.ToLower();
            filter = filter.ToLower();

            bool contains = text.Contains(filter);

            
            // Todo: yes I know this is only doing 1 check with the filter, I'll add more later
            return contains;
        }
    }
}
