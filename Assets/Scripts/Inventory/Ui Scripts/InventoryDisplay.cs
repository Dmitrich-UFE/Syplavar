using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public abstract class InventoryDisplay : MonoBehaviour
{
    [SerializeField] MouseInv mouseInv;
    protected InventorySystem _inventorySystem;
    protected Dictionary<InventorySlotUI, InventorySlot> _slotDictionary;

    public InventorySystem InventorySystem => _inventorySystem;
    public Dictionary<InventorySlotUI, InventorySlot> SlotDictionary => _slotDictionary;

    protected virtual void Start()
    {

    }

    public abstract void AssignSlot(InventorySystem invToDisplay);

    protected virtual void UpdateSlot(InventorySlot updatedSlot)
    {
        foreach (var slot in _slotDictionary)
        {
            if (slot.Value == updatedSlot)
            {
                slot.Key.UpdateUISlot(updatedSlot);
            }
        }
    }

    public void SlotClicked(InventorySlotUI clickedUISlot)
    {
        //Берем предмет в мышь
        if (clickedUISlot.AssignedInventorySlot.ItemData != null && mouseInv.AssignedInventorySlot.ItemData == null)
        {
            mouseInv.UpdateMouseSlot(clickedUISlot.AssignedInventorySlot);
            clickedUISlot.ClearSlot();
            return;
        }

        //Кладем предмет с мыщи в пустой слот
        if (clickedUISlot.AssignedInventorySlot.ItemData == null && mouseInv.AssignedInventorySlot.ItemData != null)
        {
            clickedUISlot.AssignedInventorySlot.AssignItem(mouseInv.AssignedInventorySlot);
            clickedUISlot.UpdateUISlot();

            mouseInv.ClearSlot();
        }

        //Рассмотреть всевозможные случаи(свап предметов, объединение и тд.)
    }
}
