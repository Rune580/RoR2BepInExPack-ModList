using System;
using UnityEngine;

namespace RoR2BepInExPack.ModListSystem.Components.ModList;

public class ModListController : MonoBehaviour
{
    public GameObject modCardPrefab;
    public RectTransform viewport;

    private ModCard[] _cards = Array.Empty<ModCard>();

    public void Awake()
    {
        InitCards();
    }

    private void InitCards()
    {
        if (!modCardPrefab || !viewport)
            return;

        if (_cards.Length > 0)
        {
            foreach (var modCard in _cards)
                DestroyImmediate(modCard.gameObject);
        }
        
        _cards = new ModCard[ModListMain.guidToModDataInfo.Count];
        int i = 0;

        foreach (var modDataInfo in ModListMain.guidToModDataInfo.Values)
        {
            _cards[i] = InstantiateCard(modDataInfo.ModData);
            i++;
        }
    }

    private ModCard InstantiateCard(ModData modData)
    {
        GameObject cardObject = Instantiate(modCardPrefab, viewport);
        ModCard modCard = cardObject.GetComponent<ModCard>();
        
        modCard.BindData(modData);

        return modCard;
    }
}
