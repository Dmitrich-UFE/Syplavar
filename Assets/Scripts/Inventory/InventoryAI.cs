using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class InventoryAI : MonoBehaviour
{
    [SerializeField] private InventorySlotAI[] inventorySlots;

    [SerializeField] private InventorySlotUIAI[] inventorySlotsUI;
    [SerializeField] private InventorySlotUIAI[] lowerInventorySlotsUI;

    private PlayerInputActions _playerInputActions;
    [SerializeField] private int _activeIndex;
    [SerializeField] private int _length;
    [SerializeField] private int _maxCountLowInventory;
    [SerializeField] private ItemData _defaultItem;

    public event System.Action<int, InventorySlotAI> OnSelectedSlotChanged;

    //инициализация
    void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        
        inventorySlots = new InventorySlotAI[_length];
        _maxCountLowInventory = _maxCountLowInventory > 10? 10 : _maxCountLowInventory; //не допускается макс. индекс для нижнего инвентаря более 9
        
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            inventorySlots[i] = new InventorySlotAI(_defaultItem, 1);
            inventorySlots[i].SetDefaultItem(_defaultItem);
            inventorySlotsUI[i].SetInventorySlot(inventorySlots[i]);
            inventorySlotsUI[i].Index = i;
        }

        for (int i = 0; i < _maxCountLowInventory; i++)
        {
            lowerInventorySlotsUI[i].SetInventorySlot(inventorySlots[i]);
        }

        DrawLowerInventory();

        for (int i = 0; i < _maxCountLowInventory; i++)
        {
            lowerInventorySlotsUI[i].RedrawUnselected();
        }

        SetActiveSlot(_activeIndex);
        LoadInventory();
    }

    //Сохранение инвентаря
    public void SaveInventory()
    {
        List<InventorySlotData> invDatas = new List<InventorySlotData>(inventorySlots.Length);

        for (int i = 0; i<inventorySlots.Length; i++)
        {
            if (inventorySlots[i] != null)
            {
                InventorySlotData data = new InventorySlotData
                {ItemID = inventorySlots[i].ItemData.ItemID, count = inventorySlots[i].StackSize, SlotIndex = i};
                invDatas.Add(data);
            }
        }

        InventorySaveSystem.SaveInventory(invDatas);
    }

    //Загрузка инвентаря
    public void LoadInventory()
    {
        List<InventorySlotData> invDatas = InventorySaveSystem.LoadInventory();

        if (inventorySlots == null) inventorySlots = new InventorySlotAI[_length];

        if (invDatas.Count > 0)
        {
            foreach (InventorySlotData data in invDatas)
            {
                AddToInventory(ItemManager.GetItemDataByID(data.ItemID), data.count, data.SlotIndex);
            }
        }

        DrawLowerInventory();
    }

    //отрисовка нижнего инвентаря
    internal void DrawLowerInventory()
    {
        for (int i = 1; i < _maxCountLowInventory; i++)
        {
            lowerInventorySlotsUI[i].Redraw();
        }
    }

    //отрисовка всего инвентаря
    internal void DrawInventory()
    {
        for (int i = 1; i < _length; i++)
        {
            inventorySlotsUI[i].Redraw();
        }
    }


    //Добавляет айтемы. Возвращает true, если получилось добавить ВСЁ
    internal bool AddToInventory(ItemData itemToAdd, int count)
    {
        if (itemToAdd == null || CheckFreePlaces(itemToAdd) < count) return false;

        //Добавление предметов в уже существующие ячейки с предметами
        for (int i = 1; i < _length; i++)
        {
            if (CheckCountOfItem(itemToAdd) <= 0) break;
            if (count <= 0) break;
            
            if (inventorySlots[i].ItemData == itemToAdd) count = inventorySlots[i].AddToSlot(count);
            else continue;
            
        }

        //Добавление предметов в пустые ячейки
        for (int i = 1; i < _length; i++)
        {
            if (count <= 0) break;
            
            if (inventorySlots[i].ItemData == _defaultItem) inventorySlots[i].SetItem(itemToAdd, 0);
            if (inventorySlots[i].ItemData != itemToAdd) continue;
            count = inventorySlots[i].AddToSlot(count);
            
        }

        DrawLowerInventory();
        OnSelectedSlotChanged?.Invoke(_activeIndex, GetActiveItem());
        EventManager.SendEvent("GETITEM", itemToAdd.ItemID);
        return true;
    }

    //Добавляет предмет строго в определённую ячейку. Возвращает true, если получилось добавить всё
    internal bool AddToInventory(ItemData itemToAdd, int count, int index)
    {
        if (index >= inventorySlots.Length) return false;

        if (itemToAdd.ItemID != inventorySlots[index].ItemData.ItemID)
        {
            inventorySlots[index].SetItem(itemToAdd, 0);
        }

        count = inventorySlots[index].AddToSlot(count);

        return count <= 0;
    }

    //Добавляет предмет по ID. Возвращает true, если получилось добавить всё
    internal bool AddToInventory(int ID, int count)
    {
        ItemData item = ItemManager.GetItemDataByID(ID);
        return AddToInventory(item, count);
    }

    //Списывает айтемы с активного слота. Возвращает true, если получилось
    internal bool UseActiveItem(int count = 1)
    {
        InventorySlotAI actItem = GetActiveItem();
        if (actItem != null && count <= CheckCountOfItem(actItem.ItemData))
        {
            if (count <= actItem.StackSize)
            {
                actItem.RemoveFromSlot(count);
            }
            else
            {
                ItemData actitemtype = GetActiveItem().ItemData;
                count = actItem.RemoveFromSlot(count);
                for (int i = 1; i < _length; i++)
                {
                    if (count > 0 && actitemtype == inventorySlots[i].ItemData)
                    {
                        count = inventorySlots[i].RemoveFromSlot(count);
                    }
                }
            }

            OnSelectedSlotChanged?.Invoke(_activeIndex, GetActiveItem());
            DrawLowerInventory();
            return true;
        }

        DrawLowerInventory();
        return false;
    }

    internal bool DebitItem(ItemData item, int count)
    {
        if (count > CheckCountOfItem(item)) return false;

        foreach (var itemSlot in inventorySlots)
        {
            if (item.ItemID == itemSlot.ItemData.ItemID)
            {
                count = itemSlot.RemoveFromSlot(count);
            }
            if (count <= 0) break;
        }

        //DrawInventory();
        return true;
    }


    //Проверяет кол-во поданного предмета
    internal int CheckCountOfItem(ItemData item)
    {
        int count = 0;

        for (int i = 1; i < _length; i++)
        {
            if (item != null && item == inventorySlots[i].ItemData)
            {
                count += inventorySlots[i].StackSize;
            }
        }

        return count;
    }

    //Проверяет кол-во поданного предмета по его ID
    internal int CheckCountOfItemByID(int ID)
    {
        int count = 0;

        for (int i = 1; i < _length; i++)
        {
            if (ID == inventorySlots[i].ItemData.ItemID)
            {
                count += inventorySlots[i].StackSize;
            }
        }

        return count;
    }

    //Проверяет кол-во свободного места для этого айтема
    internal int CheckFreePlaces(ItemData item)
    {
        int count = 0;

        for (int i = 1; i < _length; i++)
        {
            if (inventorySlots[i].ItemData == _defaultItem)
            {
                count += inventorySlots[i].ItemData.MaxStack;
            }
            else if (item == inventorySlots[i].ItemData)
            {
                count += inventorySlots[i].ItemData.MaxStack - inventorySlots[i].StackSize;
            }
        } 

        return count;
    }

    //меняет элементы местами по индексу
    internal void SwapElements(int sendIndex, int recIndex)
    {
        InventorySlotAI invSlot = inventorySlots[recIndex];
        inventorySlots[recIndex] = inventorySlots[sendIndex];
        inventorySlots[sendIndex] = invSlot;

        inventorySlotsUI[recIndex].SetInventorySlot(inventorySlots[recIndex]);
        inventorySlotsUI[sendIndex].SetInventorySlot(inventorySlots[sendIndex]);

        DrawInventory();

        if (sendIndex < _maxCountLowInventory)
            lowerInventorySlotsUI[sendIndex].SetInventorySlot(inventorySlots[sendIndex]);
        if (recIndex < _maxCountLowInventory)
            lowerInventorySlotsUI[recIndex].SetInventorySlot(inventorySlots[recIndex]);
        

        DrawLowerInventory();
        OnSelectedSlotChanged?.Invoke(_activeIndex, GetActiveItem());
    }


    //событие прокрутки мыши для выбора слота
    private void MouseScroll(InputAction.CallbackContext context)
    {
        // Считываем значение прокрутки
        Vector2 scrollValue = context.ReadValue<Vector2>();

        if (scrollValue.y == 0) return;

        // Определяем направление: -1 если вверх, 1 если вниз
        int direction = scrollValue.y > 0 ? -1 : 1;

        int newActiveIndex = _activeIndex + direction;
        if (newActiveIndex < 0) newActiveIndex += _maxCountLowInventory;
        // Вызываем метод смены слота
        SetActiveSlot((newActiveIndex) % _maxCountLowInventory);
    }


    //переключает слот по нажатию цифры клавиатуры
    private void KeyboardScroll(InputAction.CallbackContext context)
    {
        string keyPressed = context.control.name;

        if (int.TryParse(keyPressed, out int number))
        {
            SetActiveSlot(number);
        }
    }


    //меняет выбранный элемент из нижнего инвентаря по индексу
    private void SetActiveSlot(int index)
    {
        if (index < 0 || index >= _maxCountLowInventory) return;

        lowerInventorySlotsUI[_activeIndex].RedrawUnselected();
        lowerInventorySlotsUI[index].RedrawSelected();
        _activeIndex = index;

        OnSelectedSlotChanged?.Invoke(_activeIndex, GetActiveItem());
    }

    //получение слота с содердимым
    internal InventorySlotAI GetActiveItem() { return inventorySlots[_activeIndex];}


    private void OnEnable()
    {
        _playerInputActions.Player.MouseScroll.performed += MouseScroll;
        _playerInputActions.Player.SelectItemByKeyboard.performed += KeyboardScroll;
        _playerInputActions.Enable();
    }

    private void OnDisable()
    {
        _playerInputActions.Player.MouseScroll.performed -= MouseScroll;
        _playerInputActions.Player.SelectItemByKeyboard.performed -= KeyboardScroll;
        _playerInputActions.Disable();
    }
}



