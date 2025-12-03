using UnityEngine;
using System.Collections.Generic;

public class StaticInventoryDisplay : InventoryDisplay
{
    [SerializeField] private InventoryHolder _inventoryHolder;
    [SerializeField] private InventorySlotUI[] _slots;

    protected override void Start()
    {
        base.Start();

        if (_inventoryHolder != null)
        {
            _inventorySystem = _inventoryHolder.InventorySystem;
            _inventorySystem.OnInventorySlotChanged += UpdateSlot;
        }
        else Debug.LogWarning($"No inventory assigned to {this.gameObject}");

        AssignSlot(_inventorySystem);
    }
    public override void AssignSlot(InventorySystem invToDisplay)
    {
        _slotDictionary = new Dictionary<InventorySlotUI, InventorySlot>();

        if (_slots.Length != _inventorySystem.InventorySize) Debug.Log($"Не синхронизированы слоты инвентаря здесь {this.gameObject}");

        for (int i = 0; i < _inventorySystem.InventorySize; i++)
        {
            _slotDictionary.Add(_slots[i], _inventorySystem.InventorySlots[i]);
            _slots[i].Init(InventorySystem.InventorySlots[i]);
        }
    }
}
