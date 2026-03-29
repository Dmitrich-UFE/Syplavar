using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUIAI : MonoBehaviour
{
    [SerializeField] private Image _itemSprite;
    [SerializeField] private TextMeshProUGUI _itemCountText;
    [SerializeField] private Image _selectionSprite;

    internal InventorySlotAI MySlot {get; private set;}
    internal int Index {get; set;}

    

    //отрисовка ячейки
    internal void Redraw()
    {
        _itemSprite.sprite = MySlot.ItemData.Texture;
        _itemCountText.text = MySlot.StackSize > 1 ? MySlot.StackSize.ToString() : "";
    }

    //отрисовка того, что предмет выбран
    internal void RedrawSelected()
    {
        _selectionSprite.enabled = true;
    }

    //отрисовка того, что предмет НЕ выбран
    internal void RedrawUnselected()
    {
        _selectionSprite.enabled = false;
    }

    //Замена инвентори слота 
    internal void SetInventorySlot(InventorySlotAI slot)
    {
        if (slot == null) return;
        MySlot = slot;
    }
}
