using UnityEngine;

public interface IItem
{
    Texture2D Texture { get; set; }
    string Name { get; set; }
    GameObject GameObject { get; }
}
