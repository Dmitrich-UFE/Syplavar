using UnityEngine;
using System.Collections.Generic;

public class StaticInventoryDisplay : InventoryDisplay
{
    [SerializeField] private InventoryHolder _inventoryHolder;
    [SerializeField] private InventorySlotUI[] _slots;

    private int _currentSelectedIndex = 0;

    public event System.Action<int, InventorySlot> OnSelectedSlotChanged;

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

        if (_slots.Length > 0)
        {
            InitializeHotbarSelection();
        }
    }

    private void Update()
    {
        MouseScroll();
        KeyboardScroll();
    }

    // выбор слота клавиатурой
    private void KeyboardScroll()
    {
        for (int i = 0; i < Mathf.Min(9, _slots.Length); i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SetSelectedSlot(i);
                return;
            }

            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                SetSelectedSlot(9);
            }
        }
    }


    // Выбор слота колесом мыши
    private void MouseScroll()
    {
        float scrollValue = Input.mouseScrollDelta.y;

        if (Mathf.Abs(scrollValue) > 0.1f)
        {
            if (scrollValue > 0)
            {
                SetSelectedSlot(_currentSelectedIndex - 1);
            }
            else
            {
                SetSelectedSlot(_currentSelectedIndex + 1);
            }
        }
    }

    public void SetSelectedSlot(int index)
    {
        if (_slots.Length == 0) return;

        // Бесконечный скролл
        if (index < 0) index = _slots.Length - 1;
        else if (index >= _slots.Length) index = 0;

        // Выключение активного слота
        _slots[_currentSelectedIndex].SetSelected(false);

        // Включение активного слота
        _currentSelectedIndex = index;
        _slots[_currentSelectedIndex].SetSelected(true);

        OnSelectedSlotChanged?.Invoke(_currentSelectedIndex, GetSelectedSlot());
    }

    private void InitializeHotbarSelection()
    {
        ClearAllSelections();

        // Первый слот выбран по умолчанию
        if (_slots.Length > 0)
        {
            _currentSelectedIndex = 0;
            _slots[_currentSelectedIndex].SetSelected(true);
        }
    }

    private void ClearAllSelections()
    {
        foreach (var slot in _slots)
        {
            slot.SetSelected(false);
        }
    }

    public override void AssignSlot(InventorySystem invToDisplay)
    {
        _slotDictionary = new Dictionary<InventorySlotUI, InventorySlot>();

        if (_slots.Length != _inventorySystem.InventorySize) Debug.Log($"Не синхронизированы слоты инвентаря здесь {this.gameObject}");

        for (int i = 0; i < _inventorySystem.InventorySize; i++)
        {
            _slotDictionary.Add(_slots[i], _inventorySystem.InventorySlots[i]);
            _slots[i].Init(InventorySystem.InventorySlots[i]);

            _slots[i].SetSelected(i == _currentSelectedIndex);
        }
    }

    public InventorySlot GetSelectedSlot()
    {
        if (_currentSelectedIndex >= _slots.Length)
            return null;

        return _slots[_currentSelectedIndex].AssignedInventorySlot;
    }
}
