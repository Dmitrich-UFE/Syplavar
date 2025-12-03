using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class InventoryHolder : MonoBehaviour
{
    [SerializeField] private int _inventorySize;
    [SerializeField] protected InventorySystem _inventorySystem;

    public InventorySystem InventorySystem => _inventorySystem;

    public static UnityAction<InventorySystem> OnDynamicInventoryDisplayRequested;

    public void Awake()
    {
        _inventorySystem = new InventorySystem(_inventorySize);
    }
}
