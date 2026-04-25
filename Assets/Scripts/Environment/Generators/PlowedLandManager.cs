using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public class PlowedLandManager : MonoBehaviour
{
    private static int _IDforNewPlant;
    internal static PlowedLandManager instance {get; private set;}
    internal Dictionary<int, PlowedLandData> PlowedLands {get; private set;}
    [SerializeField] private GameObject plowedLandPrefab;
    [SerializeField] private GameObject[] plantPrefabs;
   
    void Awake()
    {
        instance = this;
        PlowedLands = new Dictionary<int, PlowedLandData>();
        LoadPlowedLands();
    }

    public void LoadPlowedLands()
    {
        List<PlowedLandData> landDatas = SaveLoadPlowedLand.LoadLands();
       
        if (landDatas != null && landDatas.Count > 0)
        {
            foreach (PlowedLandData landData in landDatas)
            {
                GameObject plowedLandObj = Instantiate(plowedLandPrefab, landData.Position, Quaternion.identity);
                
                GameObject iplant = GetCorrectPlant(landData);

                landData.Plant = iplant != null? Instantiate(iplant, plowedLandObj.transform) : null;
                PlowedLand plowedLand = plowedLandObj.GetComponent<PlowedLand>();

                plowedLand.Init(landData);
                PlowedLands[landData.ID] = landData;
                _IDforNewPlant = Math.Max(_IDforNewPlant, landData.ID);
            }
        }
    }

    public void SavePlowedLands()
    {
        List<PlowedLandData> landDatas = new List<PlowedLandData>(PlowedLands.Count);
        foreach (PlowedLandData landData in PlowedLands.Values)
        {
            if (landData != null && landData.ID >= 0)
            {
                landData.Plant = null;
                //landData.ID = 0;
                landDatas.Add(landData);
            }
        }

        SaveLoadPlowedLand.SaveLands(landDatas);
    }


    private GameObject GetCorrectPlant(PlowedLandData data)
    {
        if (data.Type == PlantTypes.NullPlant) return null;

        foreach (var plant in plantPrefabs)
        {
            IPlant iplant = plant.GetComponent<IPlant>();
            if (iplant.Type == data.Type)
            {
                return plant;
            }
        }

        return null;
    }

    public static void Update(PlowedLandData data)
    {
        if (data != null)
        {
            if (data.ID < 0)
            {
                instance.PlowedLands.Remove(-data.ID);
                Debug.LogWarning($"Изменен объект: {data.ID}, в слваре: {instance.PlowedLands.Count}");
                return;
            }
            instance.PlowedLands[data.ID] = data;
        }
    }

    internal static int GetID()
    {
        _IDforNewPlant++;
        return _IDforNewPlant;
    }
    
}

public static class SaveLoadPlowedLand
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "plowedLands.json");

    public static void SaveLands(List<PlowedLandData> lands)
    {
        PlowedLandDataWrapper wrapper = new PlowedLandDataWrapper { Lands = lands };
        string json = JsonUtility.ToJson(wrapper, true); // true для красивого форматирования
        File.WriteAllText(SavePath, json);
        Debug.Log($"Сохранено в: {SavePath}");
    }

    public static List<PlowedLandData> LoadLands()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("Файл сохранения не найден");
            return new List<PlowedLandData>(0);
        }

        string json = File.ReadAllText(SavePath);
        PlowedLandDataWrapper wrapper = JsonUtility.FromJson<PlowedLandDataWrapper>(json);
        Debug.Log("Файл с грядками загружен");
        return wrapper.Lands;
    }
}

[System.Serializable]
public class PlowedLandData
{
    public int ID;
    public PlantTypes Type;
    public Vector3Int Position;
    public bool Wet;
    public PlantStatus PlantStatus;
    public GameObject Plant;
}

[System.Serializable]
public class PlowedLandDataWrapper
{
    public List<PlowedLandData> Lands;
}

public enum PlantTypes
{
    NullPlant, Cabbage, Cucumber, Tomato, Wheat, IronFlower, Potato, Beet
}