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
    public TextAsset modDescription;
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

    internal void CreateModData() => new ModData(modGUIDIdentifier, modDescription.text, modIcon, links);
}
