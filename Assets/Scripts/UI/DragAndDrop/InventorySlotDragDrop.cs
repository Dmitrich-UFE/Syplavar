using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotDragDrop : MonoBehaviour, IDropHandler
{

    [SerializeField] internal InventorySlotUIAI _invSlotUI {get; private set;}
    private InventoryAI _invAI;
    
    void Start()
    {
        _invSlotUI = GetComponent<InventorySlotUIAI>();
        _invAI = GetInventoryAI.getInventoryAI();
    }

    public void OnDrop(PointerEventData eventData)
    {
        DraggableItem draggedItem = eventData.pointerDrag.GetComponent<DraggableItem>();
        
        if (draggedItem != null && draggedItem.sourceSlot != null)
        {
            _invAI.SwapElements(draggedItem.sourceSlot._invSlotUI.Index, _invSlotUI.Index);
        }
    }
}