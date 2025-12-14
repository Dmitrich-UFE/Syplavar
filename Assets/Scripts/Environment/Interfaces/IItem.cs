using UnityEngine;

public interface IItem
{
    Sprite Texture { get; }
    string Name { get; }
    int MaxStack { get; }
    GameObject GameObject { get; }
}
