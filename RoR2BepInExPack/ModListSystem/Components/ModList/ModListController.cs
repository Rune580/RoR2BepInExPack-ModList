using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2BepInExPack.ModListSystem.Components.ModList;

public class ModListController : MonoBehaviour
{
    public GameObject modCardPrefab;
    public ScrollRect targetScrollRect;
    public RectOffset padding;
    public float spacing;
    public int maxPoolSize = 10;
    public float coverageMultiplier = 1.5f;
    public float recyclingThreshold = 0.2f;

    private readonly RecycleArray<ModCard> _cardPool = new();
    private readonly Vector3[] _corners = new Vector3[4];
    private ModDataInfo[] _mods;
    private RectTransform _viewport;
    private RectTransform _content;
    private Vector2 _prevAnchoredPos;
    private Bounds _viewBounds = new();

    public void Awake()
    {
        if (!targetScrollRect)
            return;

        if (!modCardPrefab)
            return;
        
        _viewport = targetScrollRect.viewport;

        if (!_viewport)
            return;
        
        _viewport.GetWorldCorners(_corners);
        float threshold = recyclingThreshold * (_corners[2].y - _corners[0].y);
        
        _viewBounds.min = new Vector3(_corners[0].x, _corners[0].y - threshold);
        _viewBounds.max = new Vector3(_corners[2].x, _corners[2].y + threshold);

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

        targetScrollRect.onValueChanged.AddListener(HandleScroll);
    }

    public void UpdateDataSet(ModDataInfo[] mods)
    {
        if (mods.Length == 0)
            return;
        
        _mods = mods;

        float cardHeight = modCardPrefab.GetComponent<RectTransform>().rect.size.y;
        float contentHeight = (spacing * (_mods.Length - 1)) + (cardHeight * _mods.Length);

        _content.sizeDelta = new Vector2(_content.sizeDelta.x, contentHeight + padding.bottom);
        
        SetupPool();
    }

    private void SetupPool()
    {
        _cardPool.Clear();

        float requiredCoverage = coverageMultiplier * _viewport.rect.height;
        int minPoolSize = Mathf.Min(_mods.Length, maxPoolSize);

        float currentCoverage = 0;
        float posY = padding.top;

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
        
        // Todo: check if a card in the pool goes out of the view bounds and recycle it.
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
        
        internal RecycleArray()
        {
            _pool = new List<TItem>();
            _topIndex = 0;
            _bottomIndex = 0;
        }

        internal void Add(TItem item)
        {
            _pool.Add(item);

            _bottomIndex = _pool.Count - 1;
        }

        internal TItem RecycleTopToBottom()
        {
            _bottomIndex = _topIndex;
            _topIndex = (_topIndex + 1) % _pool.Count;

            return _pool[_bottomIndex];
        }

        internal TItem RecycleBottomToTop()
        {
            _topIndex = _bottomIndex;
            _bottomIndex = (_bottomIndex - 1 + _pool.Count) % _pool.Count;

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
        }
    }
}
