using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System;

public class InventoryAI : MonoBehaviour
{
    [SerializeField] private InventorySlotAI[] inventorySlots;

    [SerializeField] private InventorySlotUIAI[] inventorySlotsUI;
    [SerializeField] private InventorySlotUIAI[] lowerInventorySlotsUI;

    //private PlayerInputActions _playerInputActions;
    private Vector2 _mouseMoveInput;
    [SerializeField] private int _activeIndex;
    [SerializeField] private int _length;
    [SerializeField] private int _maxCountLowInventory;
    public UnityAction<InventorySlot> OnInventorySlotChanged;

    public event System.Action<int, InventorySlotAI> OnSelectedSlotChanged;

    //инициализация
    void Awake()
    {
        //_playerInputActions = new PlayerInputActions();
        //_playerInputActions.Player.MouseScroll.performed += context => _mouseMoveInput = context.ReadValue<Vector2>();
        //_playerInputActions.Player.MouseScroll.canceled += context => _mouseMoveInput = Vector2.zero;

        inventorySlots = new InventorySlotAI[_length];
        _maxCountLowInventory = _maxCountLowInventory > 10? 10 : _maxCountLowInventory; //не допускается макс. индекс для нижнего инвентаря более 9

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            inventorySlots[i] = new InventorySlotAI();
            inventorySlotsUI[i].SetInventorySlot(inventorySlots[i]);
        }

        DrawLowerInventory();
        SetActiveSlot(_activeIndex);
    }

    internal void DrawLowerInventory()
    {
        for (int i = 0; i < _maxCountLowInventory; i++)
        {
            lowerInventorySlotsUI[i].Redraw();
        }
    }

    //событие прокрутки мыши для выбора слота
    public void MouseScroll(InputAction.CallbackContext context)
    {
        // Считываем значение прокрутки
        Vector2 scrollValue = context.ReadValue<Vector2>();

        if (scrollValue.y == 0) return;

        // Определяем направление: 1 если вверх, -1 если вниз
        int direction = scrollValue.y > 0 ? 1 : -1;

        // Вызываем метод смены слота
        SetActiveSlot((_activeIndex + direction) % _maxCountLowInventory);
    }

    //переключает слот по нажатию цифры клавиатуры
    private void KeyboardScroll()
    {
        for (int i = 0; i < _maxCountLowInventory; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SetActiveSlot(i);
                return;
            }

            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                SetActiveSlot(_maxCountLowInventory - 1);
            }
        }
    }

    //меняет выбранный элемент из нижнего инвентаря по индексу
    internal void SetActiveSlot(int index)
    {
        if (index < 0 || index >= _maxCountLowInventory - 1) return;

        lowerInventorySlotsUI[_activeIndex].RedrawUnselected();
        lowerInventorySlotsUI[index].RedrawSelected();
        _activeIndex = index;
    }

    internal InventorySlotAI GetActiveItem() { return inventorySlots[_activeIndex];}
}
