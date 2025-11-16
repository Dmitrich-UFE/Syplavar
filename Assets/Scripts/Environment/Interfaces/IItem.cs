using UnityEngine;

public interface IItem
{
    Texture2D Texture { get; }
    string Name { get; }
    int MaxStack { get; }
    GameObject GameObject { get; }
}
