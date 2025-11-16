using UnityEngine;

[CreateAssetMenu(menuName = "Inventory System/Inventory Item")]
public class ItemData : ScriptableObject, IItem
{
    [SerializeField] private Texture2D _texture;
    [SerializeField] private string _name;
    [SerializeField] private int _maxStack;
    [SerializeField] private GameObject _gameObject;

    public Texture2D Texture => _texture;
    public string Name => _name;
    public int MaxStack => _maxStack;
    public GameObject GameObject => _gameObject;
}
