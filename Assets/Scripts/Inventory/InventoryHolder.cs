using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class InventoryHolder : MonoBehaviour
{
    [SerializeField] private int _inventorySize;
    [SerializeField] protected InventorySystem _inventorySystem;
    [SerializeField] private ItemData _defaultItem;

    public InventorySystem InventorySystem => _inventorySystem;

    public static UnityAction<InventorySystem> OnDynamicInventoryDisplayRequested;

    public void Awake()
    {
        InventorySlot.SetDefaultItem(_defaultItem);

        _inventorySystem = new InventorySystem(_inventorySize);
    }
}
