using System;

namespace RoR2BepInExPack.ModListSystem.Markdown.Images;

internal class ImageHeaderInspector
{
    public const int MaxHeaderLength = 11;
    private readonly byte[] _headerBytes;
    private int _pos;

    public ImageHeaderInspector(byte[] data)
    {
        _headerBytes = new byte[MaxHeaderLength];
        Array.Copy(data, _headerBytes, MaxHeaderLength);
    }
    
    public bool MatchAndMoveForward(params byte[] bytes)
    {
        if (bytes.Length + _pos >= MaxHeaderLength)
            return false;
        
        var foundStart = false;
        
        for (int i = 0; i < bytes.Length; i++)
        {
            var byteToFind = bytes[i];

            while (_pos < MaxHeaderLength && !foundStart)
            {
                foundStart = byteToFind == _headerBytes[_pos];
                _pos++;

                if (foundStart)
                {
                    i++;
                    byteToFind = bytes[i];
                }
            }

            if (!foundStart)
                return false;

            if (byteToFind != _headerBytes[_pos])
                return false;

            _pos++;
            if (_pos >= MaxHeaderLength)
                return false;
        }

        return true;
    }

    public byte GetCurrent() => _headerBytes[_pos];

    public bool MatchForward(ReadOnlySpan<byte> bytes) => MatchForward(bytes.ToArray());

    public bool MatchForward(params byte[] bytes)
    {
        var origPos = _pos;
        
        var matched = MatchAndMoveForward(bytes);
        
        _pos = origPos;

        return matched;
    }

    public void Decrement(int count)
    {
        _pos -= count;
        
        if (_pos < 0)
            _pos = 0;
    }

    public void Reset() => _pos = 0;
}
