using UnityEngine;

public class InventorySlotAI
{
    [SerializeField] private ItemData _itemData;
    [SerializeField] private int _stackSize;
    private ItemData _defaultItem;

    public ItemData ItemData => _itemData;
    public int StackSize => _stackSize;

    //конструктор
    internal InventorySlotAI(ItemData source, int amount)
    {
        _itemData = source;
        _stackSize = amount;
    }

    //меняет предмет на новый. Возвращает кол-во добавленных предметов
    internal void SetItem(ItemData item, int amount)
    {
        _itemData = item;
        _stackSize = amount;
    }

    internal void SetDefaultItem(ItemData item)
    {
        _defaultItem = item;
    }

    internal InventorySlotAI()
    {
        Clear();
    }

    //очистка слота
    internal void Clear()
    {
        _itemData = null;
        _stackSize = 0;
    }

    //проверка, сколько осталось места в слоте. Возвращает кол-во свободных штук
    internal int CheckHowLeftInSlot()
    {return _itemData.MaxStack - _stackSize;}

    //Добавляет некоторое кол-во предметов. Возвращает кол-во предметов, которые нужно добавить
    internal int AddToSlot(int amount)
    {

        if (_itemData.MaxStack < _stackSize + amount)
        {
            int _size = _stackSize;
            _stackSize = _itemData.MaxStack;
            return _size + amount - _itemData.MaxStack;
        }

        _stackSize += amount;
        return 0;
    }

    //Списывает некоторое кол-во предметов. Возвращает кол-во предметов, которое еще нужно списать
    internal int RemoveFromSlot(int amount)
    {
        if (_stackSize <= amount)
        {
            int _size = _stackSize;
            SetItem(_defaultItem, 1);
            return amount - _size;
        }

        _stackSize -= amount;
        return 0;
    }

    //Является ли слот пустым?
    internal bool isEmpty()
    {
        return _itemData == null || _itemData.Name == _defaultItem.Name;
    }
    
}
