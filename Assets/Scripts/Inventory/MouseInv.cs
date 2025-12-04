using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class MouseInv : MonoBehaviour
{
    public Image ItemSprite;
    public TextMeshProUGUI ItemCount;
    public InventorySlot AssignedInventorySlot;

    private void Awake()
    {
        ItemSprite.color = Color.clear;
        ItemCount.text = "";
    }

    public void UpdateMouseSlot(InventorySlot slot)
    {
        AssignedInventorySlot.AssignItem(slot);
        ItemSprite.sprite = slot.ItemData.Texture;
        ItemCount.text = slot.StackSize.ToString();
        ItemSprite.color = Color.white;
    }

    private void Update()
    {
        if (!AssignedInventorySlot.IsEmpty())
        {
            transform.position = Mouse.current.position.ReadValue();

        }
    }

    public void ClearSlot()
    {
        AssignedInventorySlot.ClearSlot();
        ItemCount.text = "";
        ItemSprite.color = Color.clear;
        ItemSprite.sprite = null;
    }
}
