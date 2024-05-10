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

    internal void AddEmoji(string codePoint, VectorGraphic emojiGraphic)
    {
        _codePointEmojiLut ??= new Dictionary<string, VectorGraphic>();
        _codePointEmojiLut[codePoint] = emojiGraphic;
    }

    internal void Clear()
    {
        _codePointEmojiLut ??= new Dictionary<string, VectorGraphic>();
        _codePointEmojiLut.Clear();
        
        _serializedCodePointEmojiLut = [];
    }

    public VectorGraphic this[string codePoint] => _codePointEmojiLut[codePoint];

    public bool TryGetEmoji(string codePoint, out VectorGraphic emojiGraphic) =>
        _codePointEmojiLut.TryGetValue(codePoint, out emojiGraphic);

    public void OnBeforeSerialize()
    {
        _codePointEmojiLut ??= new Dictionary<string, VectorGraphic>();

        _serializedCodePointEmojiLut = _codePointEmojiLut
            .Where(kvp => !string.IsNullOrEmpty(kvp.Key) && kvp.Value)
            .Select(kvp => new SerializedDict(kvp.Key, kvp.Value))
            .ToArray();
    }

    public void OnAfterDeserialize()
    {
        _serializedCodePointEmojiLut ??= [];
        
        _codePointEmojiLut = _serializedCodePointEmojiLut
            .Where(kvp => !string.IsNullOrEmpty(kvp.key) && kvp.value)
            .ToDictionary(kvp => kvp.key, kvp => kvp.value);
    }

    [Serializable]
    private struct SerializedDict(string key, VectorGraphic value)
    {
        public string key = key;
        public VectorGraphic value = value;
    }
}
