using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image _itemSprite;
    [SerializeField] private TextMeshProUGUI _itemCount;
    [SerializeField] private InventorySlot _assignedInventorySlot;
    [SerializeField] private Image _selectionSprite;

    private Button button;

    public InventorySlot AssignedInventorySlot => _assignedInventorySlot;
    public InventoryDisplay ParentDisplay { get; private set; }

    private void Awake()
    {
        ClearSlot();

        button = GetComponent<Button>();
        button?.onClick.AddListener(OnUISlotClick);

        ParentDisplay = transform.parent.GetComponent<InventoryDisplay>();

        SetSelected(false);
    }

    public void Init(InventorySlot slot)
    {
        _assignedInventorySlot = slot;
        UpdateUISlot(slot);
    }

    public void UpdateUISlot(InventorySlot slot)
    {
        if (!slot.IsEmpty())
        {
            _itemSprite.sprite = slot.ItemData.Texture;
            _itemSprite.color = Color.white;

            if (slot.StackSize > 1)
            {
                _itemCount.text = slot.StackSize.ToString();
            }
            else
            {
                _itemCount.text = "";
            }
        }
        else
        {
            ClearSlot();
        }
    }

    public void UpdateUISlot()
    {
        if (_assignedInventorySlot != null)
        {
            UpdateUISlot(_assignedInventorySlot);
        }
    }

    public void OnUISlotClick()
    {
        ParentDisplay?.SlotClicked(this);
    }

    // ¬кл/выкл рамки дл€ выбранного слота
    public void SetSelected(bool selected)
    {
        if (_selectionSprite != null)
        {
            _selectionSprite.enabled = selected;
        }
    }

    public void ClearSlot()
    {
        _assignedInventorySlot?.ClearSlot();
        _itemSprite.sprite = null;
        _itemSprite.color = Color.clear;
        _itemCount.text = "";
    }
}
