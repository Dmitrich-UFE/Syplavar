using UnityEngine;

[CreateAssetMenu(menuName = "Inventory System/Inventory Item")]
public class ItemData : ScriptableObject, IItem
{
    [SerializeField] private Sprite _texture;
    [SerializeField] private string _name;
    [SerializeField] private int _maxStack;
    [SerializeField] private GameObject _gameObject;

    public Sprite Texture => _texture;
    public string Name => _name;
    public int MaxStack => _maxStack;
    public GameObject GameObject => _gameObject;

    //public void SetGameObject(GameObject go)
    //{
    //    _gameObject = go;
    //}

    //public IItem Copy()
    //{
    //    IItem item = Instantiate(this);
    //    item.SetGameObject(Instantiate(GameObject));
    //    return item;
    //}

    
}
