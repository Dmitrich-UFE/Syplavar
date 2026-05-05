using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;

public class TaskManager : MonoBehaviour
{
    [SerializeField] private Task[] tasks;
    [SerializeField] private int index;
    [SerializeField] private int status;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private InventoryAI inventory;
    [SerializeField] private PlowedLandManager plowedLandManager;

    [Header("STORY UI")]
    [SerializeField] private TMP_Text taskNameText;
    [SerializeField] private TMP_Text taskGoalText;
    [SerializeField] private TMP_Text taskHintText;


    //EventManager;
    internal string TaskHintText {get => taskHintText.text; set { taskHintText.text = value;}}
    internal int Status {get => status; set { status = value; StorySaveSystem.SaveStory(new StorySaveData{index = index, status = Status});}}


    void Awake()
    {
        StorySaveData data = StorySaveSystem.LoadStory();

        if (data != null)
        {
            index = data.index;
            Status = data.status; 
            if (index >= 0 && index < tasks.Length)
            {
                TaskHintText = "";
                taskNameText.text = tasks[index].Name;
                taskGoalText.text = tasks[index].GoalDescription;
                tasks[index].Activate();
            }
            else if (index == tasks.Length)
            {

            }
            else
            {
                index = 0;
                TaskHintText = "";
                taskNameText.text = tasks[index].Name;
                taskGoalText.text = tasks[index].GoalDescription;
                tasks[index].Activate();
            }
        }
        else
        {
            index = 0;
            TaskHintText = "";
            taskNameText.text = tasks[index].Name;
            taskGoalText.text = tasks[index].GoalDescription;
            tasks[index].Activate();
        }
    }

    internal void CompleteTask(int ID)
    {
        if (tasks[index].ID == ID)
        {
            tasks[index].Complete();
            index++;
            StorySaveSystem.SaveStory(new StorySaveData{index = index, status = Status});

            playerManager.SpawnPointPosition = playerManager.PlayerPosition;
            playerManager.SaveData();
            inventory.SaveInventory();
            plowedLandManager.SavePlowedLands();
            DayLightHandlerManager.Save();
            
            if (index < tasks.Length)
            {
                TaskHintText = "";
                taskNameText.text = tasks[index].Name;
                taskGoalText.text = tasks[index].GoalDescription;
                tasks[index].Activate();
            }
        }
    }

    internal bool isStoryCompleted()
    {
        return index == tasks.Length;
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
    public int status;
}
