using UnityEngine;
using Unity.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class InventorySlot
{
    [SerializeField] private ItemData _itemData;
    [SerializeField] private int _stackSize;

    public ItemData ItemData => _itemData;
    public int StackSize => _stackSize;
    
    public InventorySlot(ItemData source, int amount)
    {
        _itemData = source;
        _stackSize = amount;
    }

    public InventorySlot()
    {
        ClearSlot();
    }

    public void AssignItem(InventorySlot slot)
    {
        if (_itemData == slot.ItemData)
        {
            AddToStack(slot.StackSize);
        }
        else
        {
            _itemData = slot.ItemData;
            _stackSize = 0;
            AddToStack(slot.StackSize);
        }
    }

    public void UpdateInventorySlot(ItemData data, int amount)
    {
        _itemData = data;
        _stackSize = amount;
    }

    public bool RoomLeftInStack(int amountToAdd, out int amountRemaining)
    {
        amountRemaining = ItemData.MaxStack - _stackSize;
        return RoomLeftInStack(amountToAdd);
    }

    public bool RoomLeftInStack(int amountToAdd)
    {
        if (_stackSize + amountToAdd <= _itemData.MaxStack) return true;
        else return false;
    }

    public void AddToStack(int amount)
    {
        _stackSize += amount;
    }

    public void RemoveFromStack(int amount)
    {
        _stackSize -= amount;
    }

    public void ClearSlot()
    {
        _itemData = null;
        _stackSize = -1;
    }
}
