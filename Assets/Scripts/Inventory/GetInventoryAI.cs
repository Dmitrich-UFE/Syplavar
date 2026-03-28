using UnityEngine;

public class GetInventoryAI : MonoBehaviour
{
    private static InventoryAI _staticInventoryAI;
    [SerializeField] private GameObject _inventoryAI;

    void Awake()
    {
        _staticInventoryAI = _inventoryAI.GetComponent<InventoryAI>();;
    }

    internal static InventoryAI getInventoryAI()
    {
        return _staticInventoryAI;
    }
}
