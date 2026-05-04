using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class TaskManager : MonoBehaviour
{
    [SerializeField] private Task[] tasks;
    [SerializeField] private int index;
    //EventManager;


    void Awake()
    {
        StorySaveData data = StorySaveSystem.LoadStory();

        if (data != null)
        {
            index = data.index;
            tasks[index].Activate();
        }
    }

    void CompleteTask(int ID)
    {
        if (tasks[index].ID == ID)
        {
            tasks[index].Complete();
            index++; //
            tasks[index].Activate();
        }
    }
}

public static class StorySaveSystem
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "Story.json");

    public static void SaveStory(StorySaveData data)
    {
        string json = JsonUtility.ToJson(data, true); // true для красивого форматирования
        File.WriteAllText(SavePath, json);
        Debug.Log($"Сохранено в: {SavePath}");
    }

    public static StorySaveData LoadStory()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("Файл сохранения не найден");
            return null;
        }

        string json = File.ReadAllText(SavePath);
        StorySaveData data = JsonUtility.FromJson<StorySaveData>(json);
        Debug.Log("Файл с сюжетом загружен");
        return data;
    }
}


[System.Serializable]
public class StorySaveData
{
    public int index;
}
