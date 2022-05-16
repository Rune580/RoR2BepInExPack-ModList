using System;
using System.Reflection;
using MonoMod.RuntimeDetour;
using RoR2.UI;
using RoR2.UI.MainMenu;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2BepInExPack.ModListSystem;

internal static class MenuModifications
{
    private static Hook _mainMenuHook;

    internal static void InitHooks()
    {
        var targetMethod = typeof(MainMenuController).GetMethod(nameof(MainMenuController.Awake),
            BindingFlags.NonPublic | BindingFlags.Instance);
        var destMethod = typeof(MenuModifications).GetMethod(nameof(MainMenuControllerOnAwake),
            BindingFlags.NonPublic | BindingFlags.Static);

        _mainMenuHook = new Hook(targetMethod, destMethod);
    }

    private static void MainMenuControllerOnAwake(Action<MainMenuController> orig, MainMenuController self)
    {
        AttachModsButtonToMainMenu(self.titleMenuScreen);
        orig(self);
    }

    private static void AttachModsButtonToMainMenu(BaseMainMenuScreen titleMenu)
    {
        Transform juiceTransform = titleMenu.transform.Find("SafeZone/GenericMenuButtonPanel/JuicePanel");
        
        GameObject settingsButtonObject = juiceTransform.Find("GenericMenuButton (Settings)").gameObject;
        GameObject splitInstance = UnityObject.Instantiate(AssetReferences.SplitButton, juiceTransform);
        GameObject modsButtonObject = splitInstance.transform.Find("GenericMenuButton (Mods)").gameObject;
        
        HGButton modsButton = modsButtonObject.GetComponent<HGButton>();
        modsButton.hoverLanguageTextMeshController = settingsButtonObject.GetComponent<HGButton>().hoverLanguageTextMeshController;
        modsButton.updateTextOnHover = true;

        modsButtonObject.GetComponent<Image>().type = Image.Type.Sliced;
        
        splitInstance.transform.SetSiblingIndex(settingsButtonObject.transform.GetSiblingIndex());

        var layoutElement = settingsButtonObject.AddComponent<LayoutElement>();
        layoutElement.minWidth = 160;
        layoutElement.minHeight = 48;
        layoutElement.preferredWidth = 160;
        layoutElement.preferredWidth = 160;
        
        settingsButtonObject.transform.SetParent(splitInstance.transform);
        settingsButtonObject.transform.SetAsFirstSibling();
        settingsButtonObject.transform.localScale = Vector3.one;
    }
}
