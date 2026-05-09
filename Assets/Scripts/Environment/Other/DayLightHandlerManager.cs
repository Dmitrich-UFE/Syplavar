using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class DayLightHandlerManager : MonoBehaviour
{
    [SerializeField] private DayLightHandler dayHandler;
    private static DayLightHandlerManager instance;
    void Awake()
    {
        instance = this;
        Load();
    }

    public static void Save()
    {
        DayLightHandlerSaveData data = instance.dayHandler.GetData();
        DayLightHandlerSaveSystem.SaveDayLightHandler(data);
    }

    public static void Load()
    {
        DayLightHandlerSaveData data = DayLightHandlerSaveSystem.LoadDayLightHandler();
        instance.dayHandler.LoadData(data);
    }
}

public static class DayLightHandlerSaveSystem
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "dayLightHandler.json");

    public static void SaveDayLightHandler(DayLightHandlerSaveData data)
    {
        string json = JsonUtility.ToJson(data, true); // true для красивого форматирования
        File.WriteAllText(SavePath, json);
        Debug.Log($"Сохранено в: {SavePath}");
    }

    public static DayLightHandlerSaveData LoadDayLightHandler()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("Файл сохранения не найден");
            return null;
        }

        string json = File.ReadAllText(SavePath);
        DayLightHandlerSaveData data = JsonUtility.FromJson<DayLightHandlerSaveData>(json);
        Debug.Log("Файл с обработчиком дня загружен");
        return data;
    }
}


[System.Serializable]
public class DayLightHandlerSaveData
{
    public float DayMoment;
    public bool Cloudy;
    public bool IsSleepTime;
    public List<dayTime> Times;
}

[System.Serializable]
public struct dayTime 
{
    public int hh; 
    public int mm; 
    public bool isReached;
}
