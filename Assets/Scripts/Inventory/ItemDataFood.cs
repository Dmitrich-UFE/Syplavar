using UnityEngine;

[CreateAssetMenu(menuName = "Inventory System/Inventory Item Food")]
public class ItemDataFood : ItemData
{
    [SerializeField] private int _addingMind;
    [SerializeField] private int _addingHealth;

    public int AddingMind => _addingMind;
    public int AddingHealth => _addingHealth;

}
