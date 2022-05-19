using System;
using System.Collections.Generic;
using System.Text;
using BepInEx;
using UnityEngine;

namespace RoR2BepInExPack.ModListSystem;

[CreateAssetMenu(menuName = "RoR2BepinexPack/ModData")]

public class SerializableModData : ScriptableObject
{
    [Tooltip($"The GUID of the mod this ModData belongs to.")]
    public string modGUIDIdentifier;
    [Tooltip($"If your BaseUnityPlugin reverts all changes when it gets disabled (IE: BetterUI), you can set this to true so your mod can be disabled at runtime. If you dont know what this means, leave this false.")]
    public bool supportsRuntimeToggling;
    public TextAsset modDescription; // Should replace this with a description token
    public Sprite modIcon;
    public HyperLink[] links;

    internal readonly static List<SerializableModData> instances = new List<SerializableModData>();

    private void OnEnable()
    {
        instances.Add(this);
    }
    private void OnDisable()
    {
        instances.Remove(this);
    }
}
