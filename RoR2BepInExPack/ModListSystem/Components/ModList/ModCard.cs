using RoR2.UI;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2BepInExPack.ModListSystem.Components.ModList;

public class ModCard : HGButton
{
    public Image modIcon;
    public GameObject iconFallback;
    public HGTextMeshProUGUI nameLabel;
    public LanguageTextMeshController descriptionLanguageController;

    private ModData _boundData;

    public override void Awake()
    {
        if (_boundData != null)
            ReloadFromData();
        
        base.Awake();
    }

    public override void OnEnable()
    {
        if (_boundData != null)
            ReloadFromData();
        
        base.OnEnable();
    }

    public void BindData(ModData modData)
    {
        _boundData = modData;
        ReloadFromData();
    }

    private void ReloadFromData()
    {
        if (_boundData.Icon)
        {
            iconFallback.SetActive(false);
            modIcon.sprite = _boundData.Icon;
            modIcon.enabled = true;
        }
        else
        {
            iconFallback.SetActive(true);
            modIcon.enabled = false;
        }
        
        nameLabel.SetText(_boundData.Name);
        
        descriptionLanguageController.token = _boundData.DescriptionToken;
    }
}
