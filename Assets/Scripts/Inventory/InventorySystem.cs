using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

[System.Serializable]
public class InventorySystem
{
    [SerializeField] private List<InventorySlot> _inventory;

    public List<InventorySlot> InventorySlots => _inventory;
    public int InventorySize => _inventory.Count;

    //public UnityAction<InventorySlot> OnInventorySlotChanged;

    public InventorySystem(int size) 
    {
        _inventory = new List<InventorySlot>(size);

        for (int i = 0; i < size; i++)
        {
            _inventory.Add(new InventorySlot());
        }
    }

    public bool AddToInventory(ItemData itemToAdd, int amountToAdd)
    {
        if (ContainsItem(itemToAdd, out List<InventorySlot> inventorySlots))
        {
            foreach (var slot in inventorySlots)
            {
                if (slot.RoomLeftInStack(amountToAdd))
                {
                    slot.AddToStack(amountToAdd);
                    //OnInventorySlotChanged?.Invoke(slot);
                    return true;
                }
            }
        }
        
        if (HasFreeSlot(out InventorySlot freeSlot))
        {
            freeSlot.UpdateInventorySlot(itemToAdd, amountToAdd);
            //OnInventorySlotChanged?.Invoke(freeSlot);
            return true;
        }
        return false;
    }

    public bool ContainsItem(ItemData itemToAdd, out List<InventorySlot> inventorySlots)
    {
        inventorySlots = InventorySlots.Where(i => i.ItemData == itemToAdd).ToList();
        return inventorySlots == null ? false : true;
    }

    public bool HasFreeSlot(out InventorySlot freeSlot)
    {
        freeSlot = InventorySlots.FirstOrDefault(i => i.ItemData == null);
        return freeSlot == null ? false : true;
    }
}
