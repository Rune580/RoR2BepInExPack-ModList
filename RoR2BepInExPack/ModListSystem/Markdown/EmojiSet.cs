using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("ModListSystem.Editor")]

namespace RoR2BepInExPack.ModListSystem.Markdown;

[CreateAssetMenu]
public class EmojiSet : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField, HideInInspector]
    private SerializedDict[] _serializedCodePointEmojiLut;
    private Dictionary<string, VectorGraphic> _codePointEmojiLut;

    internal void AddEmoji(string unicode, VectorGraphic emojiGraphic)
    {
        _codePointEmojiLut[unicode] = emojiGraphic;
    }

    // public VectorGraphic this[string codePoint]
    // {
    //     get
    //     {
    //         Char.
    //     }
    // }
    
    public void OnBeforeSerialize()
    {
        _codePointEmojiLut ??= new Dictionary<string, VectorGraphic>();

        _serializedCodePointEmojiLut = _codePointEmojiLut.Select(kvp => new SerializedDict(kvp.Key, kvp.Value))
            .ToArray();
    }

    public void OnAfterDeserialize()
    {
        _serializedCodePointEmojiLut ??= [];
        
        _codePointEmojiLut = _serializedCodePointEmojiLut.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    [Serializable]
    private readonly struct SerializedDict(string key, VectorGraphic value)
    {
        public readonly string Key = key;
        public readonly VectorGraphic Value = value;
    }
}
