using System;
using RoR2.UI;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2BepInExPack.ModListSystem.Components.ModList;

public class ModCard : HGButton
{
    public Image modIcon;
    public GameObject iconFallback;
    public HGTextMeshProUGUI nameLabel;
    public HGTextMeshProUGUI versionLabel;
    public HGTextMeshProUGUI authorLabel;
    public LanguageTextMeshController descriptionLanguageController;
    
    [HideInInspector] public RectTransform rectTransform;

    private ModData _boundData;

    public override void Awake()
    {
        if (!rectTransform)
            rectTransform = GetComponent<RectTransform>();
        
        if (_boundData != null)
            ReloadFromData();

        base.Awake();
    }

    public override void OnEnable()
    {
        if (!rectTransform)
            rectTransform = GetComponent<RectTransform>();
        
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
        ResetState();
        
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

        if (_boundData.Author != "Unknown Author")
        {
            authorLabel.enabled = true;
            authorLabel.SetText(_boundData.Author);
        }
        else
        {
            authorLabel.enabled = false;
        }
        
        nameLabel.SetText(_boundData.Name);
        versionLabel.SetText(_boundData.Version.ToString());
        
        descriptionLanguageController.token = _boundData.DescriptionToken;
    }

    private void ResetState()
    {
        try
        {
            OnDeselect(null);
            DoStateTransition(SelectionState.Normal, true);
        }
        catch (NullReferenceException)
        {
            
        }
    }
}
