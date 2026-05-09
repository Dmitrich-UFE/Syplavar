using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class ItemManager : MonoBehaviour
{
    [SerializeField] private ItemData[] items;
    private static ItemManager instance;

    void Awake(){instance = this;}
    
    internal static ItemData GetItemDataByID(int ID)
    {
        foreach(var item in ItemManager.instance.items)
        {
            if (item.ItemID == ID)
                return item;
        }
        return null;
    } 
}

public static class InventorySaveSystem
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "playerInventory.json");

    public static void SaveInventory(List<InventorySlotData> slots)
    {
        InventoryDataWrapper wrapper = new InventoryDataWrapper { Slots = slots };
        string json = JsonUtility.ToJson(wrapper, true); // true для красивого форматирования
        File.WriteAllText(SavePath, json);
        Debug.Log($"Сохранено в: {SavePath}");
    }

    public static List<InventorySlotData> LoadInventory()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("Файл сохранения не найден");
            return new List<InventorySlotData>(0);
        }

        string json = File.ReadAllText(SavePath);
        InventoryDataWrapper wrapper = JsonUtility.FromJson<InventoryDataWrapper>(json);
        Debug.Log("Файл с инвентарём загружен");
        return wrapper.Slots;
    }
}

[System.Serializable]
public class InventorySlotData
{
    public int ItemID;
    public int count;
    public int SlotIndex;
}

[System.Serializable]
public class InventoryDataWrapper
{
    public List<InventorySlotData> Slots;
}
